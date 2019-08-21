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

    private EnergySphere CacheEnergySphere = new EnergySphere();

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
                //注意顺序是先收集当前的球,然后再生成3个，再释放技能
                CollectSphere(CurSphereInfo);
                for(int i = 0; i < 3; ++i)
                {
                    CustomEnergySphere();
                }

                //忽略小概率事件，确保本地调用
                NetClass.AllPlayerInfo[NetClass.LocalPlayer].GetComponent<PlayerSkillController>().ReleaseSkill(CurSphereInfo.Type, _playerid);
                return;
            }

            //否则执行普通的收集逻辑
            CollectSphere(CurSphereInfo);
        }
    }


    private void CollectSphere(SphereInfo CurSphereInfo)
    {
        _EnergyList.Add(CurSphereInfo);
        int CurIndex = CurSphereInfo.SphereId;
        CacheEnergySphere.PlayerId = _playerid;
        CacheEnergySphere.SphereId = CurIndex;
        NetClass.SendDataToServer(CacheEnergySphere, (int)Protocal.MESSAGE_COLLECT);
        SphereManager.Collect(CurIndex);
    }


    public bool CustomEnergySphere()
    {
        //当处于技能时间内不能消耗能量球
        if (SkillController.SuperTime > 0f)
            return true;

        if (_EnergyList.Count == 0)
            return false;
        SphereInfo CustomSphere = _EnergyList[_EnergyList.Count - 1];
        CacheEnergySphere.PlayerId = _playerid;
        CacheEnergySphere.SphereId = CustomSphere.SphereId;
        UpdateVec.X = Random.Range(-width, width);
        UpdateVec.Y = Random.Range(-height, height);
        LocalVec.x = UpdateVec.X;
        LocalVec.y = UpdateVec.Y;
        CacheEnergySphere.Position = UpdateVec;
        NetClass.SendDataToServer(CacheEnergySphere, (int)Protocal.MESSAGE_GENERATORENERGY);
        SphereManager.GeneratorNewSphere(CustomSphere.SphereId, LocalVec);
        _EnergyList.RemoveAt(_EnergyList.Count - 1);
        return true;
    }
}
