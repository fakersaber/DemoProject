using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergyController : MonoBehaviour
{
    static float width = 17.2f;
    static float height = 9.3f;
    private NetWorkManager NetClass;
    private EnergySpherePool SphereManager;
    private YVector2 UpdateVec;
    private Vector2 LocalVec;
    private PlayerSkillController SkillController;
    private EnergyUIManager _uIManager;
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


    public EnergyUIManager uIManager
    {
        get { return _uIManager; }
    }

    private void Awake()
    {
        GameObject GameManager = GameObject.FindWithTag("GameManager");
        NetClass = GameManager.GetComponent<NetWorkManager>();
        SphereManager = GameManager.GetComponent<EnergySpherePool>();
        SkillController = GetComponent<PlayerSkillController>();
        _uIManager = GetComponentInChildren<EnergyUIManager>();
        UpdateVec = new YVector2();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //存在一个球两次进入OnTriggerEnter2D函数..
        if (NetClass.LocalPlayer == 1)
        {
            if (_EnergyList.Count == 3)
                return;

            SphereInfo CurSphereInfo = collision.gameObject.GetComponent<SphereInfo>();
            if (!SphereManager._AllSpherePoll[CurSphereInfo.SphereId].activeSelf)
                return;

            CollectSphere(CurSphereInfo);
            if (_playerid == NetClass.LocalPlayer)
            {
                switch (_EnergyList.Count)
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

            if (_EnergyList.Count == 3
                && _EnergyList[0].Type == _EnergyList[1].Type 
                && _EnergyList[1].Type == _EnergyList[2].Type)
            {
                NetClass.AllPlayerInfo[_playerid].GetComponent<PlayerSkillController>().ReleaseSkill(CurSphereInfo.Type, _playerid);
            }
        }
    }


    private void CollectSphere(SphereInfo CurSphereInfo)
    {
        //当处于技能时间内不能收集能量球
        if (SkillController.SuperTime > 0f)
            return;
        _EnergyList.Add(CurSphereInfo);
        CacheEnergySphere.PlayerId = _playerid;
        CacheEnergySphere.SphereId = CurSphereInfo.SphereId;
        CacheEnergySphere.Type = CurSphereInfo.Type;
        NetClass.SendDataToServer(CacheEnergySphere, (int)Protocal.MESSAGE_COLLECT);
        SphereManager.Collect(CurSphereInfo.SphereId);
        _uIManager.CollectSphere(CurSphereInfo.Type);
    }


    public bool ConsumeEnergySphere()
    {
        //当处于技能时间内不能消耗能量球
        if (SkillController.SuperTime > 0f)
            return true;
        if (_EnergyList.Count == 0)
            return false;
        ConsumeEnergySphere2();
        return true;
    }


    public void ConsumeEnergySphere2()
    {
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
        _uIManager.ConsumeSphere();
    }
}
