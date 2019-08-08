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
    public int LocalPlayer;
    private Socket LocalSocket;
    private const int ServerPort = 8000;
    private const string ServerIp = "127.0.0.1";
    private Thread recvThread;
    private GameObject otherPlayer;
    private AccrossThreadHelper HelperClass;




    //每个玩家自己对应的发送内容
    public Dictionary<int, UpdateInfo> AllPlayerInfo;

    void Start()
    {
        HelperClass = AccrossThreadHelper.Instance;
        AllPlayerInfo = new Dictionary<int, UpdateInfo>();
        Connect();
        StartCoroutine(Send());
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


    IEnumerator Send()
    {
        //硬编码，玩家id都是1到roomsize
        while (true)
        {
            for(int index = 1;index <= AllPlayerInfo.Count; ++index)
            {
                if (AllPlayerInfo[index] != null)
                {
                    SendDataToServer(AllPlayerInfo[index]);
                    AllPlayerInfo[index] = null;
                }
            }
            yield return new WaitForSeconds(0.04f);
        }
    }


    private void RecvThread()
    {
        byte[] readBuff = new byte[1024];
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
                        case (int)Protocal.MESSAGE_CREATEOBJ:
                            HandleCreateObject(CreateObjInfo.Parser.ParseFrom(readBuff, offset + sizeof(int) * 2, size));
                            break;
                        case (int)Protocal.MESSAGE_UPDATEDATA:
                            HandleUpdateData(UpdateInfo.Parser.ParseFrom(readBuff, sizeof(int) * 2, size));
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
            //创建所有玩家的更新结构
            AllPlayerInfo.Add(message.PlayerId, null);

            GameObject NewObject =  Instantiate(InitPrefab,new Vector3(message.Position.X, message.Position.Y, 0f),Quaternion.Euler(0f,0f,message.Rotation));
            if (message.IsManclient)
            {
                NewObject.AddComponent<LocalPlayerController>();
                LocalPlayer = message.PlayerId;
            }
            else
            {
                otherPlayer = NewObject;
                otherPlayer.AddComponent<PlayerController>();
            }
        }); 
    }



    private void HandleUpdateData(UpdateInfo message)
    {
        //更新
        HelperClass.AddDelegate(() =>{
            Rigidbody2D TargetRigidbody = otherPlayer.GetComponent<Rigidbody2D>();
            PlayerController Controller = otherPlayer.GetComponent<PlayerController>();

            //更新插值的值
            Controller.StartSynchronizepos = TargetRigidbody.position;
            Controller.EndSynchronizepos = new Vector2(message.Position.X, message.Position.Y);
            Controller.StartSynchronizerot = TargetRigidbody.rotation;
            Controller.EndSynchronizerot = message.Rotation;
            Controller.NetCurScale = 0.1f;
        });
    }



    public void SendDataToServer(UpdateInfo SendClass)
    {
        try
        {
            byte[] sendbuffer;
            using (MemoryStream stream = new MemoryStream())
            {
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
