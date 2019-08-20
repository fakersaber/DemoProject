using UnityEngine;
using System.Collections;

public class PlayerSkillController : MonoBehaviour
{
    private NetWorkManager NetClass;
    private Rigidbody2D PlayerRigidbody;

    private SkillInfo UpdateSkillInfo = new SkillInfo();
    private YVector2 UpdateVec = new YVector2();

    public float ChaosTime = 0f; //混乱时间
    public float FreezeTime = 0f; //冰冻时间
    public float ThunderTime = 0f; //强化时间



    public GameObject ChaosEffectObj;
    public GameObject ThunderEffectObj;
    private ParticleSystem _ChaosEffect;
    private ParticleSystem _ThunderEffect;


    public ParticleSystem ChaosEffect
    {
        get { return _ChaosEffect; }
        set { _ChaosEffect = value; }
    }

    public ParticleSystem ThunderEffect
    {
        get { return _ThunderEffect; }
        set { _ThunderEffect = value; }
    }

    private void Awake()
    {
        GameObject GameManager = GameObject.FindWithTag("GameManager");
        NetClass = GameManager.GetComponent<NetWorkManager>();
        PlayerRigidbody = GetComponent<Rigidbody2D>();
        _ChaosEffect = ChaosEffectObj.GetComponent<ParticleSystem>();
        _ThunderEffect = ThunderEffectObj.GetComponent<ParticleSystem>();
    }



    //该函数仅仅被localplayer调用，第二个参数为技能释放者
    public void ReleaseSkill(int type,int PalyerId)
    {
        UpdateSkillInfo.Type = type;
        UpdateVec.X = PlayerRigidbody.position.x;
        UpdateVec.Y = PlayerRigidbody.position.y;
        UpdateSkillInfo.PlayerId = PalyerId;
        NetClass.SendDataToServer(UpdateSkillInfo,(int)Protocal.MESSAGE_RELEASESKILL);

        var ReleasePlayer = NetClass.AllPlayerInfo[PalyerId].GetComponent<PlayerSkillController>();
        if (type == (int)SphereType.SPHERE_YELLOW)
        {
            ReleasePlayer.PlaySkillEffect(type);
            ReleasePlayer.AddSkillTime(type);
            return;
        }
        for (int i = 1; i <= NetClass.AllPlayerInfo.Count; ++i)
        {
            if (i != PalyerId)
            {
                NetClass.AllPlayerInfo[i].GetComponent<PlayerSkillController>().AddSkillTime(type);
                NetClass.AllPlayerInfo[i].GetComponent<PlayerSkillController>().PlaySkillEffect(type);
            }
        }
    }


    public void AddSkillTime(int type)
    {
        switch (type)
        {
            case (int)SphereType.SPHERE_RED:
                ChaosTime = 10f;
                break;
            case (int)SphereType.SPHERE_BLUE:
                FreezeTime = 10f;
                break;
            case (int)SphereType.SPHERE_YELLOW:
                ThunderTime = 10f;
                break;
        }
    }


    //同步仅仅调用该函数
    public void PlaySkillEffect(int type)
    {
        switch (type)
        {
            case (int)SphereType.SPHERE_RED:
                _ChaosEffect.Play();
                break;
            case (int)SphereType.SPHERE_BLUE:

                break;
            case (int)SphereType.SPHERE_YELLOW:
                _ThunderEffect.Play();
                break;
        }
    }
}
