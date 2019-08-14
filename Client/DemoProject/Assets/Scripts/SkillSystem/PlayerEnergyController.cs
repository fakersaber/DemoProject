using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergyController                                                                                           : MonoBehaviour
{
    static float width = 12.8f;
    static float height = 7.2f;
    private NetWorkManager NetClass;
    private EnergySpherePool SphereManager;
    private YVector2 UpdateVec;
    private Vector2 LocalVec;
    private int _playerid;


    private Stack<SphereInfo> _EnergyStack = new Stack<SphereInfo>(3);
    private float _SpurtTime = 0f;

    public float SpurtTime
    {
        get { return _SpurtTime; }
        set { _SpurtTime = value; }
    }

    public Stack<SphereInfo> EnergyStack
    {
        get { return _EnergyStack; }
    }

    public int playerid
    {
        get { return _playerid; }
        set { _playerid = value; }
    }




    public void SetSpurtTime()
    {
        _SpurtTime = 0.5f;
    }


    private void Awake()
    {
        GameObject GameManager = GameObject.FindWithTag("GameManager");
        NetClass = GameManager.GetComponent<NetWorkManager>();
        SphereManager = GameManager.GetComponent<EnergySpherePool>();
        UpdateVec = new YVector2();

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (NetClass.LocalPlayer == 1)
        {
            if (_EnergyStack.Count == 3)
                return;
            SphereInfo CurSphereInfo = collision.gameObject.GetComponent<SphereInfo>();
            _EnergyStack.Push(CurSphereInfo);
            int CurIndex = CurSphereInfo.SphereId;
            EnergySphere SendClass = new EnergySphere() { PlayerId = _playerid, SphereId = CurIndex };
            //其他客户端在接收时添加对应player的能量栈
            NetClass.SendDataToServer(SendClass, (int)Protocal.MESSAGE_COLLECT);
            SphereManager.Collect(CurIndex);
        }
    }



    public bool CustomEnergySphere()
    {
        if (_EnergyStack.Count == 0)
            return false;
        SphereInfo CustomSphere = _EnergyStack.Pop();
        EnergySphere SendClass = new EnergySphere() { PlayerId = _playerid, SphereId = CustomSphere.SphereId };
        UpdateVec.X = Random.Range(-width, width);
        UpdateVec.Y = Random.Range(-height, height);
        LocalVec.x = UpdateVec.X;
        LocalVec.y = UpdateVec.Y;
        SendClass.Position = UpdateVec;
        //其他客户端在接收时扣除对应player的能量栈
        NetClass.SendDataToServer(SendClass, (int)Protocal.MESSAGE_GENERATORENERGY);
        SphereManager.GeneratorNewSphere(CustomSphere.SphereId, LocalVec);
        return true;
    }
}
