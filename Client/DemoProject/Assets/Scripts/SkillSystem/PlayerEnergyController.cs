using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergyController : MonoBehaviour
{
    static float width = 12.8f;
    static float height = 7.2f;
    private NetWorkManager NetClass;
    private EnergySpherePool SphereManager;
    private YVector2 UpdateVec;
    private Vector2 LocalVec;
    private PlayerSkillController SkillController;

    private int _playerid;
    private List<SphereInfo> _EnergyList = new List<SphereInfo>(3);
    private float _SpurtTime = 0f;

    public float SpurtTime
    {
        get { return _SpurtTime; }
        set { _SpurtTime = value; }
    }

    public List<SphereInfo> EnergyList
    {
        get { return _EnergyList; }
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
        SkillController = GetComponent<PlayerSkillController>();
        UpdateVec = new YVector2();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (NetClass.LocalPlayer == 1)
        {
            if (_EnergyList.Count == 3)
                return;
            SphereInfo CurSphereInfo = collision.gameObject.GetComponent<SphereInfo>();
            if (_EnergyList.Count == 2 && _EnergyList[0].Type == _EnergyList[1].Type && CurSphereInfo.Type == _EnergyList[0].Type)
            {
                //释放技能啦
                SkillController.ReleaseSkill(_EnergyList[0].Type);
                _EnergyList.Clear();
                return;
            }

            #region
            _EnergyList.Add(CurSphereInfo);
            int CurIndex = CurSphereInfo.SphereId;
            EnergySphere SendClass = new EnergySphere() { PlayerId = _playerid, SphereId = CurIndex };
            //其他客户端在接收时添加对应player的能量栈
            NetClass.SendDataToServer(SendClass, (int)Protocal.MESSAGE_COLLECT);
            SphereManager.Collect(CurIndex);
            #endregion
        }
    }



    public bool CustomEnergySphere()
    {
        if (_EnergyList.Count == 0)
            return false;
        SphereInfo CustomSphere = _EnergyList[_EnergyList.Count - 1];
        EnergySphere SendClass = new EnergySphere() { PlayerId = _playerid, SphereId = CustomSphere.SphereId };
        UpdateVec.X = Random.Range(-width, width);
        UpdateVec.Y = Random.Range(-height, height);
        LocalVec.x = UpdateVec.X;
        LocalVec.y = UpdateVec.Y;
        SendClass.Position = UpdateVec;
        //其他客户端在接收时扣除对应player的能量栈
        NetClass.SendDataToServer(SendClass, (int)Protocal.MESSAGE_GENERATORENERGY);
        SphereManager.GeneratorNewSphere(CustomSphere.SphereId, LocalVec);
        _EnergyList.RemoveAt(_EnergyList.Count - 1);
        return true;
    }
}
