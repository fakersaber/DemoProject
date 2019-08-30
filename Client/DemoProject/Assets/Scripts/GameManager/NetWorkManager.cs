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

    public GameObject PinkPlayer_1;
    public GameObject RedPlayer_2;
    public GameObject PurplePlayer_3;
    public GameObject GreenPlayer_4;
    public GameObject InitCamera;
    public int LocalPlayer;
    public Dictionary<int, GameObject> AllPlayerInfo = new Dictionary<int, GameObject>(4);
    public Dictionary<int, Rigidbody2D> AllPlayerRigidy = new Dictionary<int, Rigidbody2D>(4);
    public Dictionary<int, PlayerController> AllPlayerController = new Dictionary<int, PlayerController>(3);



    private Socket LocalSocket;
    private Thread recvThread;
    private AccrossThreadHelper HelperClass;
    private Vector2 TargetPosition;
    private EnergySpherePool SpherePoll;
    private PlayerEffectsManager EffectsManager;
    private LoadingManager LocalLoadingManager;


    //cache component



    private void Awake()
    {
        SpherePoll = GetComponent<EnergySpherePool>();
        EffectsManager = GetComponent<PlayerEffectsManager>();
        LocalLoadingManager = GetComponent<LoadingManager>();
        HelperClass = AccrossThreadHelper.Instance;
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
        byte[] readBuff = new byte[1024 * 8];
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
                        case (int)Protocal.MESSAGE_RELEASESKILL:
                            HandleReleaseSkill(SkillInfo.Parser.ParseFrom(readBuff, offset + sizeof(int) * 2, size));
                            break;
                        case (int)Protocal.MESSAGE_INITENERGYSPHERE:
                            HandleInitEnergySphere(EnergySphereInit.Parser.ParseFrom(readBuff, offset + sizeof(int) * 2, size));
                            break;
                        case (int)Protocal.MESSAGE_CREATEOBJ:
                            HandleCreateObject(CreateObjInfo.Parser.ParseFrom(readBuff, offset + sizeof(int) * 2, size));
                            break;
                        case (int)Protocal.MESSAGE_LOADING:
                            HandleLoading(UpdateInfo.Parser.ParseFrom(readBuff, offset + sizeof(int) * 2, size));
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
        HelperClass.AddDelegate(() => {
            GameObject InitPrefab = null;
            switch (message.PlayerId)
            {
                case 1:
                    InitPrefab = PinkPlayer_1;
                    break;
                case 2:
                    InitPrefab = RedPlayer_2;
                    break;
                case 3:
                    InitPrefab = PurplePlayer_3;
                    break;
                case 4:
                    InitPrefab = GreenPlayer_4;
                    break;
            }
            GameObject NewObject =  Instantiate(InitPrefab,new Vector3(message.Position.X, message.Position.Y, 0f),Quaternion.Euler(0f,0f,message.Rotation));
            AllPlayerInfo.Add(message.PlayerId, NewObject);   
            EffectsManager.InitPlayerEffects(message.PlayerId);
            if (message.IsManclient)
            {
                var Controller = NewObject.AddComponent<LocalPlayerController>();
                LocalPlayer = message.PlayerId;
                GameObject.FindWithTag("Spurt").GetComponent<SpurtButton>().LocalPlayerEnergy = NewObject.GetComponent<PlayerEnergyController>();
            }
            else
            {
                var Controller = NewObject.AddComponent<PlayerController>();
                Controller.PlayerId = message.PlayerId;
            }
            NewObject.GetComponent<PlayerEnergyController>().playerid = message.PlayerId;
            NewObject.GetComponent<PlayerHealth>().PlayerId = message.PlayerId;
            AllPlayerRigidy.Add(message.PlayerId, NewObject.GetComponent<Rigidbody2D>());
            AllPlayerController.Add(message.PlayerId, NewObject.GetComponent<PlayerController>());

            LocalLoadingManager.LocalDownPlayer++;
            if (LocalLoadingManager.LocalDownPlayer == LoadingManager.RoomSize)
            {
                //任意一个结构，反正为空
                UpdateInfo SendClass = new UpdateInfo() { PlayerId = LocalPlayer };
                SendDataToServer(SendClass,(int)Protocal.MESSAGE_LOADING);
            }
        }); 
    }

    private void HandleUpdateData(UpdateInfo message)
    {
        //更新
        HelperClass.AddDelegate(() =>{
            PlayerController Controller = AllPlayerController[message.PlayerId];
            Rigidbody2D TargetRigidbody = AllPlayerRigidy[message.PlayerId];
            TargetPosition.x = message.Position.X;
            TargetPosition.y = message.Position.Y;
            //TargetRigidbody.position = TargetPosition;
            //TargetRigidbody.rotation = message.Rotation;
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
            TargetPosition.x = message.Position.X;
            TargetPosition.y = message.Position.Y;
            if (message.EffectsIndex == (int)SpecialEffects.WEAPONTOWEAPON)
                AudioController.Play("Effect5");
            else
                AudioController.Play("Effect4");
            EffectsManager.PlayerSpecialEffects(message.PlayerId,message.EffectsIndex, TargetPosition);
            Player.SubHp(message.Damage);
        });
    }

    private void HandleReflectData(UpdateInfo message)
    {
        //同步该端所有内容
        HelperClass.AddDelegate(() =>{
            Rigidbody2D TargetRigidbody = AllPlayerRigidy[message.PlayerId];
            
            //getcomponent的gc只有在没有对应组件时才会产生
            if(message.PlayerId == LocalPlayer)
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
                PlayerController OtherPlayer = AllPlayerController[message.PlayerId];
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
            var CurPlayer = AllPlayerInfo[message.PlayerId].GetComponent<PlayerEnergyController>();
            CurPlayer.EnergyList.Add(SpherePoll.GetSphereInfo(message.SphereId));
            CurPlayer.uIManager.CollectSphere(message.Type);
            SpherePoll.Collect(message.SphereId);
            if (message.PlayerId == LocalPlayer)
            {
                switch (CurPlayer.EnergyList.Count)
                {
                    case 1:
                        AudioController.Play("Effect1");
                        break;
                    case 2:
                        AudioController.Play("Effect11");
                        break;
                    case 3:
                        AudioController.Play("Effect12");
                        break;
                }
            }
        });
    }

    private void HandleGeneratorSphere(EnergySphere message)
    {
        HelperClass.AddDelegate(() => {
            var CurPlayer = AllPlayerInfo[message.PlayerId].GetComponent<PlayerEnergyController>();
            CurPlayer.EnergyList.RemoveAt(CurPlayer.EnergyList.Count - 1);
            CurPlayer.uIManager.ConsumeSphere();
            TargetPosition.x = message.Position.X;
            TargetPosition.y = message.Position.Y;
            SpherePoll.GeneratorNewSphere(message.SphereId, TargetPosition);
        });
    }

    private void HandleReleaseSkill(SkillInfo message)
    {
        HelperClass.AddDelegate(() => {
            var ReleasePlayer = AllPlayerInfo[message.PlayerId].GetComponent<PlayerSkillController>();
            ReleasePlayer.SuperTime = 5f;
            if (message.PlayerId == LocalPlayer)
                PlayerSkillController.PlaySkillAudio(message.Type);
            if (message.Type == (int)SphereType.SPHERE_RED)
            {
                ReleasePlayer.PlaySkillEffect(message.Type);
                ReleasePlayer.AddSkillTime(message.Type);
                return;
            }
            ReleasePlayer.PlaySelfEffect(message.Type);
            for (int i = 1; i <= AllPlayerInfo.Count; ++i)
            {
                if (i != message.PlayerId)
                {
                    AllPlayerInfo[i].GetComponent<PlayerSkillController>().AddSkillTime(message.Type);
                    AllPlayerInfo[i].GetComponent<PlayerSkillController>().PlaySkillEffect(message.Type);
                }
            }
        });
    }


    private void HandleLoading(UpdateInfo message)
    {
        HelperClass.AddDelegate(() =>
        {
            LocalLoadingManager.OtherDownPlayerNum++;
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

    public void SendDataToServer(SkillInfo SendClass, int protocal)
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
        //if (recvThread != null)
        //    recvThread.Abort();
        LocalSocket.Close();
    }
}
