using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;


public class NetWorkManager : MonoBehaviour
{
    private const int ServerPort = 8000;
    private const string ServerIp = "192.168.124.207";

    public GameObject InitPrefab;
    public GameObject InitCamera;
    public int LocalPlayer;
    public Dictionary<int, GameObject> AllPlayerInfo;


    private Socket LocalSocket;
    private Thread recvThread;
    private AccrossThreadHelper HelperClass;
    private GameObject MainCamera;
    private Vector2 TargetPosition;
    private EnergySpherePool SpherePoll;


    private void Awake()
    {
        SpherePoll = GetComponent<EnergySpherePool>();
        MainCamera = Instantiate(InitCamera, new Vector3(0f, 0f, -10f), Quaternion.Euler(0f, 0f, 0f));
    }

    void Start()
    {
        HelperClass = AccrossThreadHelper.Instance;
        AllPlayerInfo = new Dictionary<int, GameObject>();
        Connect();
    }


    public void Connect()
    {
        try
        {
            LocalSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint point = new IPEndPoint(IPAddress.Parse(ServerIp), ServerPort);
            LocalSocket.Connect(point);
            Debug.Log("Connect Success");
            recvThread = new Thread(new ThreadStart(RecvThread));
            recvThread.Start();
        }
        catch (Exception)
        {
            Debug.Log("Connect Error...");
        }
    }


    //可优化为函数表
    private void RecvThread()
    {
        byte[] readBuff = new byte[4096];
        while (true)
        {
            try
            {
                int len = LocalSocket.Receive(readBuff);
                int offset = 0;
                while (len != 0)
                {
                    int protocal = System.BitConverter.ToInt32(readBuff, offset);
                    int size = System.BitConverter.ToInt32(readBuff, offset + sizeof(int));
                    switch (protocal)
                    {
                        case (int)Protocal.MESSAGE_UPDATEDATA:
                            HandleUpdateData(UpdateInfo.Parser.ParseFrom(readBuff, offset + sizeof(int) * 2, size));
                            break;
                        case (int)Protocal.MESSAGE_REFLECTDATA:
                            HandleReflectData(UpdateInfo.Parser.ParseFrom(readBuff, offset + sizeof(int) * 2, size));
                            break;
                        case (int)Protocal.MESSAGE_DAMAGE:
                            HandleAttakeInfo(AttakeInfo.Parser.ParseFrom(readBuff, offset + sizeof(int) * 2, size));
                            break;
                        case (int)Protocal.MESSAGE_COLLECT:
                            HandleCollectSphere(EnergySphere.Parser.ParseFrom(readBuff, offset + sizeof(int) * 2, size));
                            break;
                        case (int)Protocal.MESSAGE_GENERATORENERGY:
                            HandleGeneratorSphere(EnergySphere.Parser.ParseFrom(readBuff, offset + sizeof(int) * 2, size));
                            break;
                        case (int)Protocal.MESSAGE_INITENERGYSPHERE:
                            HandleInitEnergySphere(EnergySphereInit.Parser.ParseFrom(readBuff, offset + sizeof(int) * 2, size));
                            break;
                        case (int)Protocal.MESSAGE_CREATEOBJ:
                            HandleCreateObject(CreateObjInfo.Parser.ParseFrom(readBuff, offset + sizeof(int) * 2, size));
                            break;
                        default:
                            break;
                    }
                    len = len - size - sizeof(int) * 2;
                    offset = offset + size + sizeof(int) * 2;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
                Debug.Log("Recive Error");
                return;
            }
        }
    }


    private void HandleCreateObject(CreateObjInfo message)
    {
        //线程安全的委托列表单例
        HelperClass.AddDelegate(() => {
            GameObject NewObject =  Instantiate(InitPrefab,new Vector3(message.Position.X, message.Position.Y, 0f),Quaternion.Euler(0f,0f,message.Rotation));
            AllPlayerInfo.Add(message.PlayerId, NewObject);
            if (message.IsManclient)
            {
                var Controller = NewObject.AddComponent<LocalPlayerController>();
                LocalPlayer = message.PlayerId;
                MainCamera.GetComponent<CameraController>().PlayerRidibody = NewObject.GetComponent<Rigidbody2D>();
            }
            else
            {
                var Controller = NewObject.AddComponent<PlayerController>();
                Controller.PlayerId = message.PlayerId;
            }
        }); 
    }

    private void HandleUpdateData(UpdateInfo message)
    {
        //更新
        HelperClass.AddDelegate(() =>{
            //每次同步位置时，若在反弹状态中，直接丢弃包
            PlayerController Controller = AllPlayerInfo[message.PlayerId].GetComponent<PlayerController>();
            Rigidbody2D TargetRigidbody = AllPlayerInfo[message.PlayerId].GetComponent<Rigidbody2D>();
            //更新插值的值
            TargetPosition.x = message.Position.X;
            TargetPosition.y = message.Position.Y;
            Controller.StartSynchronizepos = TargetRigidbody.position;
            Controller.EndSynchronizepos = TargetPosition;
            Controller.StartSynchronizerot = TargetRigidbody.rotation;
            Controller.EndSynchronizerot = message.Rotation;
            Controller.NetCurScale = 0f;
            Controller.NetPositionScale = 0f;
        });
    }

    private void HandleAttakeInfo(AttakeInfo message)
    {
        HelperClass.AddDelegate(() => {
            //每次同步位置时，若在反弹状态中，直接丢弃包
            PlayerHealth Player = AllPlayerInfo[message.PlayerId].GetComponent<PlayerHealth>();
            Player.SubHp(message.Damage);
            TargetPosition.x = message.Position.X;
            TargetPosition.y = message.Position.Y;
            Player.PlayerSpecialEffects(message.EffectsIndex, TargetPosition);
        });
    }

    private void HandleReflectData(UpdateInfo message)
    {
        //同步该端所有内容
        HelperClass.AddDelegate(() =>{
            Rigidbody2D TargetRigidbody = AllPlayerInfo[message.PlayerId].GetComponent<Rigidbody2D>();
            PlayerController OtherPlayer = AllPlayerInfo[message.PlayerId].GetComponent<PlayerController>();
            if (OtherPlayer == null)
            {
                LocalPlayerController LocalPlayer = AllPlayerInfo[message.PlayerId].GetComponent<LocalPlayerController>();
                LocalPlayer.ReflectCurScale = 0f;
                LocalPlayer.ReflectStartPosition = TargetRigidbody.position;
                LocalPlayer.ReflectEndPosition = new Vector2(message.Position.X, message.Position.Y);
                LocalPlayer.ReflectStartRotation = TargetRigidbody.rotation;
                LocalPlayer.ReflectEndRotation = message.Rotation;
            }
            else
            {
                OtherPlayer.ReflectCurScale = 0f;
                OtherPlayer.ReflectStartPosition = TargetRigidbody.position;
                OtherPlayer.ReflectEndPosition = new Vector2(message.Position.X, message.Position.Y);
                OtherPlayer.ReflectStartRotation = TargetRigidbody.rotation;
                OtherPlayer.ReflectEndRotation = message.Rotation;
            }
        });
    }

    private void HandleInitEnergySphere(EnergySphereInit message)
    {
        HelperClass.AddDelegate(() =>{
            var AllSphereInfo = message.AllSpherePoll;
            for (int i = 0; i < AllSphereInfo.Count; ++i)
            {
                TargetPosition.x = AllSphereInfo[i].Position.X;
                TargetPosition.y = AllSphereInfo[i].Position.Y;
                SpherePoll.InitPool(TargetPosition, AllSphereInfo[i].Type, AllSphereInfo[i].SphereId);
            }
        });
    }

    private void HandleCollectSphere(EnergySphere message)
    {
        HelperClass.AddDelegate(() =>{
            SpherePoll.Collect(message.SphereId);
        });
    }

    private void HandleGeneratorSphere(EnergySphere message)
    {
        HelperClass.AddDelegate(() => {
            TargetPosition.x = message.Position.X;
            TargetPosition.y = message.Position.Y;
            SpherePoll.GeneratorNewSphere(message.SphereId, TargetPosition);
        });
    }



    public void SendDataToServer(UpdateInfo SendClass, int protocal)
    {
        try
        {
            byte[] sendbuffer;
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(System.BitConverter.GetBytes(protocal), 0, sizeof(int));
                stream.Write(System.BitConverter.GetBytes(SendClass.CalculateSize()), 0, sizeof(int)); //原始数据长度
                stream.Write(SendClass.ToByteArray(), 0, SendClass.CalculateSize());
                sendbuffer = stream.ToArray();
            }
            LocalSocket.Send(sendbuffer);
        }catch(Exception e)
        {
            Debug.Log(e);
            Debug.Log("Send Error");
        }

    }

    public void SendDataToServer(AttakeInfo SendClass,int protocal)
    {
        try
        {
            byte[] sendbuffer;
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(System.BitConverter.GetBytes(protocal), 0, sizeof(int));
                stream.Write(System.BitConverter.GetBytes(SendClass.CalculateSize()), 0, sizeof(int)); //原始数据长度
                stream.Write(SendClass.ToByteArray(), 0, SendClass.CalculateSize());
                sendbuffer = stream.ToArray();
            }
            LocalSocket.Send(sendbuffer);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log("Send Error");
        }
    }

    public void SendDataToServer(EnergySphere SendClass,int protocal)
    {
        try
        {
            byte[] sendbuffer;
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(System.BitConverter.GetBytes(protocal), 0, sizeof(int));
                stream.Write(System.BitConverter.GetBytes(SendClass.CalculateSize()), 0, sizeof(int)); //原始数据长度
                stream.Write(SendClass.ToByteArray(), 0, SendClass.CalculateSize());
                sendbuffer = stream.ToArray();
            }
            LocalSocket.Send(sendbuffer);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log("Send Error");
        }
    }


    public void OnDisable()
    {
        if (recvThread != null)
            recvThread.Abort();
        LocalSocket.Close();
    }
}
