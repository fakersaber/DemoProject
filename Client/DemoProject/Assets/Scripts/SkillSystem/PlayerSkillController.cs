using UnityEngine;
using System.Collections;

public class PlayerSkillController : MonoBehaviour
{
    private NetWorkManager NetClass;
    private Rigidbody2D PlayerRigidbody;
    private Animator animator;
    
    private SkillInfo UpdateSkillInfo = new SkillInfo();
    private YVector2 UpdateVec = new YVector2();

    public float ChaosTime = 0f; //混乱时间
    public float FreezeTime = 0f; //冰冻时间
    public float ThunderTime = 0f; //强化时间

    public float SuperTime = 0f; //释放技能后进入的超级时间


    public GameObject ChaosEffectObj;
    public GameObject ThunderEffectObj;
    public GameObject SelfEffectChaosObj;
    public GameObject SelfEffectIceObj;
    private ParticleSystem _ChaosEffect;
    private ParticleSystem _ThunderEffect;
    private ParticleSystem _SelfEffectChaos;
    private ParticleSystem _SelfEffectIce;

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

    public ParticleSystem SelfEffectChaos
    {
        get { return _SelfEffectChaos; }
        set { _SelfEffectChaos = value; }
    }

    public ParticleSystem SelfEffectIce
    {
        get { return _SelfEffectIce; }
        set { _SelfEffectIce = value; }
    }


    private void Awake()
    {
        GameObject GameManager = GameObject.FindWithTag("GameManager");
        NetClass = GameManager.GetComponent<NetWorkManager>();
        PlayerRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        _ChaosEffect = ChaosEffectObj.GetComponent<ParticleSystem>();
        _ThunderEffect = ThunderEffectObj.GetComponent<ParticleSystem>();
        _SelfEffectChaos = SelfEffectChaosObj.GetComponent<ParticleSystem>();
        _SelfEffectIce = SelfEffectIceObj.GetComponent<ParticleSystem>();
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
        ReleasePlayer.SuperTime = 5f;

        if (PalyerId == NetClass.LocalPlayer)
            PlayerSkillController.PlaySkillAudio(type);

        if (type == (int)SphereType.SPHERE_RED)
        {
            ReleasePlayer.PlaySkillEffect(type);
            ReleasePlayer.AddSkillTime(type);
            return;
        }
        ReleasePlayer.PlaySelfEffect(type);
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
            case (int)SphereType.SPHERE_PURPLE:
                ChaosTime = 5f;
                break;
            case (int)SphereType.SPHERE_BLUE:
                FreezeTime = 5f;
                break;
            case (int)SphereType.SPHERE_RED:
                ThunderTime = 5f;
                break;
        }
    }


    //同步仅仅调用该函数
    public void PlaySkillEffect(int type)
    {
        switch (type)
        {
            case (int)SphereType.SPHERE_PURPLE:
                _ChaosEffect.Play();
                break;
            case (int)SphereType.SPHERE_BLUE:
                animator.SetBool("iced", true);
                break;
            case (int)SphereType.SPHERE_RED:
                _ThunderEffect.Play();
                break;
        }
    }

    public void PlaySelfEffect(int type)
    {
        switch (type)
        {
            case (int)SphereType.SPHERE_PURPLE:
                _SelfEffectChaos.Play();
                break;
            case (int)SphereType.SPHERE_BLUE:
                _SelfEffectIce.Play();
                break;
        }
    }

    static public void PlaySkillAudio(int type)
    {
        switch (type)
        {
            case (int)SphereType.SPHERE_PURPLE:
                AudioController.Play("Effect6");
                break;
            case (int)SphereType.SPHERE_BLUE:
                AudioController.Play("Effect7");
                break;
            case (int)SphereType.SPHERE_RED:
                AudioController.Play("Effect8");
                break;
        }
    }
}
