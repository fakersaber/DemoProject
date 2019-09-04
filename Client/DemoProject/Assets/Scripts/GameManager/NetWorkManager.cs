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
    private CameraController cameraController;
    private SpurtButton SpurtTouch;
    //cache component



    private void Awake()
    {
        SpherePoll = GetComponent<EnergySpherePool>();
        EffectsManager = GetComponent<PlayerEffectsManager>();
        LocalLoadingManager = GetComponent<LoadingManager>();
        cameraController = GameObject.FindWithTag("MainCamera").GetComponent<CameraController>();
        SpurtTouch = GameObject.FindWithTag("Spurt").GetComponent<SpurtButton>();
        HelperClass = AccrossThreadHelper.Instance;
    }


    private void Start()
    {
        Connect();
    }

    public void Connect()
    {
        try
        {
            LocalSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint point = new IPEndPoint(IPAddress.Parse(ServerIp), ServerPort);
            LocalSocket.Connect(point);
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
        byte[] readBuff = new byte[1024 * 16];
        int rev_offset = 0; //接收到的断流情况下的偏移
        int offset = 0;
        int len = 0;
        while (true)
        {
            try
            {
                len = LocalSocket.Receive(readBuff, rev_offset, 1024 * 16,SocketFlags.None);
                offset = 0;
                Debug.Log("recvsize: " + len);
                while (len > 0)
                {
                    //正常解析数据时才检查是否有截断，存在截断偏移时是第一次处理数据
                    if(rev_offset == 0)
                    {
                        //protocal和size 不能正常解析，将剩余字节放到下一次解析的头部
                        if (len < 8)
                        {
                            Debug.Log("save: " + len);
                            rev_offset = len;
                            for (int i = 0; i < rev_offset; ++i)
                            {
                                readBuff[i] = readBuff[offset + i];
                            }
                            len = 0;
                            offset = 0;
                            continue;
                        }

                        //检查数据是否被截断
                        int cursize = System.BitConverter.ToInt32(readBuff, offset + sizeof(int));
                        int curlen = len - sizeof(int) * 2;
                        if (cursize > curlen)
                        {
                            Debug.Log("log size: " + cursize);
                            Debug.Log("log len: " + curlen);
                            Debug.Log("log offset: " + offset);
                            //protocal和size能够正常解析，但数据被截断,
                            rev_offset = len;
                            for (int i = 0; i < rev_offset; ++i)
                            {
                                readBuff[i] = readBuff[offset + i];
                            }
                            len = 0;
                            offset = 0;
                            continue;
                        }
                    }
                    int protocal = System.BitConverter.ToInt32(readBuff, offset);
                    int size = System.BitConverter.ToInt32(readBuff, offset + sizeof(int));
                    int CurUseSize = size + sizeof(int) * 2;
                    Debug.Log("protocal: " + protocal + "  CurUseSize: " + CurUseSize);
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
                        case (int)Protocal.MESSAGE_SPURT:
                            HandleSpurtRequest(SpurtInfo.Parser.ParseFrom(readBuff, offset + sizeof(int) * 2, size));
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
                    rev_offset = 0;
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
        HelperClass.AddDelegate(() =>
        {
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
            GameObject NewObject = Instantiate(InitPrefab, new Vector3(message.Position.X, message.Position.Y, 0f), Quaternion.Euler(0f, 0f, message.Rotation));
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
                UpdateInfo SendClass = new UpdateInfo() { PlayerId = LocalPlayer };
                SendDataToServer(SendClass, (int)Protocal.MESSAGE_LOADING);
            }
        });
    }

    private void HandleUpdateData(UpdateInfo message)
    {
        //更新
        HelperClass.AddDelegate(() =>
        {

            PlayerController Controller = AllPlayerController[message.PlayerId];
            Rigidbody2D TargetRigidbody = AllPlayerRigidy[message.PlayerId];
            if (Controller == null || TargetRigidbody == null)
            {
                Debug.Log("Controller or TargetRigidbody is null");
                return;
            }
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
        HelperClass.AddDelegate(() =>
        {
            //每次同步位置时，若在反弹状态中，直接丢弃包
            PlayerHealth Player = AllPlayerInfo[message.PlayerId].GetComponent<PlayerHealth>();
            if (Player == null)
            {
                Debug.Log("PlayerHealth is null");
                return;
            }
            TargetPosition.x = message.Position.X;
            TargetPosition.y = message.Position.Y;
            if (message.EffectsIndex == (int)SpecialEffects.WEAPONTOWEAPON)
                AudioController.Play("Effect5");
            else if(message.EffectsIndex == (int)SpecialEffects.WEAKTOWEAPON)
                AudioController.Play("Effect13");
            else
                AudioController.Play("Effect4");
            EffectsManager.PlayerSpecialEffects(message.PlayerId, message.EffectsIndex, TargetPosition);
            Player.SubHp(message.Damage);
        });
    }

    private void HandleReflectData(UpdateInfo message)
    {
        //同步该端所有内容
        HelperClass.AddDelegate(() =>
        {
            Rigidbody2D TargetRigidbody = AllPlayerRigidy[message.PlayerId];
            if (TargetRigidbody == null)
            {
                Debug.Log("HandleReflectData is null");
                return;
            }
            //getcomponent的gc只有在没有对应组件时才会产生
            if (message.PlayerId == LocalPlayer)
            {
                LocalPlayerController LocalPlayer = AllPlayerInfo[message.PlayerId].GetComponent<LocalPlayerController>();
                if (LocalPlayer == null)
                {
                    Debug.Log("HandleReflectData is null");
                    return;
                }
                LocalPlayer.ReflectCurScale = 0f;
                LocalPlayer.ReflectStartPosition = TargetRigidbody.position;
                LocalPlayer.ReflectEndPosition = new Vector2(message.Position.X, message.Position.Y);
                LocalPlayer.ReflectStartRotation = TargetRigidbody.rotation;
                LocalPlayer.ReflectEndRotation = message.Rotation;
            }
            else
            {
                PlayerController OtherPlayer = AllPlayerController[message.PlayerId];
                if (OtherPlayer == null)
                {
                    Debug.Log("HandleReflectData is null");
                    return;
                }
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
        HelperClass.AddDelegate(() =>
        {
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
        HelperClass.AddDelegate(() =>
        {
            var CurPlayer = AllPlayerInfo[message.PlayerId].GetComponent<PlayerEnergyController>();
            if (CurPlayer == null)
            {
                Debug.Log("HandleCollectSphere is null");
                return;
            }
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
        HelperClass.AddDelegate(() =>
        {
            var CurPlayer = AllPlayerInfo[message.PlayerId].GetComponent<PlayerEnergyController>();
            if (CurPlayer == null)
            {
                Debug.Log("PlayerEnergyController is null");
                return;
            }

            CurPlayer.EnergyList.RemoveAt(CurPlayer.EnergyList.Count - 1);
            CurPlayer.uIManager.ConsumeSphere();
            TargetPosition.x = message.Position.X;
            TargetPosition.y = message.Position.Y;
            SpherePoll.GeneratorNewSphere(message.SphereId, TargetPosition);
        });
    }

    private void HandleReleaseSkill(SkillInfo message)
    {
        HelperClass.AddDelegate(() =>
        {
            var ReleasePlayer = AllPlayerInfo[message.PlayerId].GetComponent<PlayerSkillController>();
            if(ReleasePlayer == null)
            {
                Debug.Log("PlayerSkillController is null");
                return;
            }


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


    private void HandleSpurtRequest(SpurtInfo message)
    {
        //1表示请求，2表示回复
        HelperClass.AddDelegate(() =>
        {
            var CurPlayer = AllPlayerInfo[message.PlayerId].GetComponent<PlayerEnergyController>();
            if (CurPlayer == null)
            {
                Debug.Log("SpurtRequest is null");
                return;
            }
            //没有通过不会收到回复，所以收到均可以直接操作容器
            if (message.Request == 2 && LocalPlayer != 1)
            {
                //收到回复首先设置能量球数据，所有端均需要设置
                CurPlayer.EnergyList.RemoveAt(CurPlayer.EnergyList.Count - 1);
                CurPlayer.uIManager.ConsumeSphere();
                TargetPosition.x = message.Position.X;
                TargetPosition.y = message.Position.Y;
                SpherePoll.GeneratorNewSphere(message.SphereId, TargetPosition);
                //冲刺数据仅仅针对player == local
                if (message.PlayerId == LocalPlayer)
                {
                    SpurtTouch.SpurtTime = 0.35f;
                    AudioController.Play("Effect0");
                }
            }

            //当前为主端且为请求
            else if (LocalPlayer == 1 && message.Request == 1)
            {
                if (CurPlayer.EnergyList.Count > 0)
                {
                    //同步其他玩家在主端上的操作
                    message.Request = 2; //设置为回复
                    CurPlayer.EnergyList.RemoveAt(CurPlayer.EnergyList.Count - 1);
                    CurPlayer.uIManager.ConsumeSphere();
                    TargetPosition.x = message.Position.X;
                    TargetPosition.y = message.Position.Y;
                    SpherePoll.GeneratorNewSphere(message.SphereId, TargetPosition);
                    SendDataToServer(message, (int)Protocal.MESSAGE_SPURT);
                }
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
            LocalSocket.Send(sendbuffer, sizeof(int) * 2 + SendClass.CalculateSize(), SocketFlags.None);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log("Send Error");
        }

    }

    public void SendDataToServer(AttakeInfo SendClass, int protocal)
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
            LocalSocket.Send(sendbuffer, sizeof(int) * 2 + SendClass.CalculateSize(), SocketFlags.None);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log("Send Error");
        }
    }

    public void SendDataToServer(EnergySphere SendClass, int protocal)
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
            LocalSocket.Send(sendbuffer, sizeof(int) * 2 + SendClass.CalculateSize(), SocketFlags.None);
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
            LocalSocket.Send(sendbuffer, sizeof(int) * 2 + SendClass.CalculateSize(), SocketFlags.None);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log("Send Error");
        }
    }

    public void SendDataToServer(SpurtInfo SendClass, int protocal)
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
            LocalSocket.Send(sendbuffer, sizeof(int) * 2 + SendClass.CalculateSize(), SocketFlags.None);
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
        //Application.Quit();
    }
}
