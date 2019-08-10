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


    public GameObject InitPrefab;
    private Socket LocalSocket;
    private const int ServerPort = 8000;
    private const string ServerIp = "192.168.124.207";
    private Thread recvThread;
    //private GameObject otherPlayer;
    private AccrossThreadHelper HelperClass;
    public int LocalPlayer;

    

    //每个玩家自己对应的发送内容
    public Dictionary<int, GameObject> AllPlayerInfo;

    //每次更新直接使用的缓存
    private Vector2 TargetPosition;

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


    private void RecvThread()
    {
        byte[] readBuff = new byte[4096];
        while (true)
        {
            try
            {
                int len = LocalSocket.Receive(readBuff);
                if (len == 4096)
                    Debug.Log("out!");
                int offset = 0;
                while (len != 0)
                {
                    int protocal = System.BitConverter.ToInt32(readBuff, offset);
                    int size = System.BitConverter.ToInt32(readBuff, offset + sizeof(int));
                    switch (protocal)
                    {
                        case (int)Protocal.MESSAGE_CREATEOBJ:
                            HandleCreateObject(CreateObjInfo.Parser.ParseFrom(readBuff, offset + sizeof(int) * 2, size));
                            break;
                        case (int)Protocal.MESSAGE_UPDATEDATA:
                            HandleUpdateData(UpdateInfo.Parser.ParseFrom(readBuff, offset + sizeof(int) * 2, size));
                            break;
                        case (int)Protocal.MESSAGE_REFLECTDATA:
                            HandleReflectData(UpdateInfo.Parser.ParseFrom(readBuff, offset + sizeof(int) * 2, size));
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




    public void OnDisable()
    {
        if (recvThread != null)
            recvThread.Abort();
        LocalSocket.Close();
    }
}
