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



    private void Awake()
    {
        GameObject GameManager = GameObject.FindWithTag("GameManager");
        NetClass = GameManager.GetComponent<NetWorkManager>();
        PlayerRigidbody = GetComponent<Rigidbody2D>();
    }



    //该函数仅仅被localplayer调用，第二个参数为技能释放者
    public void ReleaseSkill(int type,int PalyerId)
    {
        UpdateSkillInfo.Type = type;
        UpdateVec.X = PlayerRigidbody.position.x;
        UpdateVec.Y = PlayerRigidbody.position.y;
        UpdateSkillInfo.PlayerId = PalyerId;
        NetClass.SendDataToServer(UpdateSkillInfo,(int)Protocal.MESSAGE_RELEASESKILL);

        //负面技能,只要不是主端释放的就会造成影响
        if (type != (int)SphereType.SPHERE_YELLOW && PalyerId != NetClass.LocalPlayer)
        {
            AddSkillTime(type);
        }
        //增益技能，只有当是主端释放才会对主端造成影响
        else if (type == (int)SphereType.SPHERE_YELLOW && PalyerId == NetClass.LocalPlayer)
        {
            AddSkillTime(type);
        }

        //找到对应的释放者播放特效，主端在本地计算
        var SkillController = NetClass.AllPlayerInfo[PalyerId].GetComponent<PlayerSkillController>();
        SkillController.PlaySkillEffect(type);
    }


    public void AddSkillTime(int type)
    {
        switch (type)
        {
            case (int)SphereType.SPHERE_RED:
                ChaosTime = 100f;
                break;
            case (int)SphereType.SPHERE_BLUE:
                FreezeTime = 100f;
                break;
            case (int)SphereType.SPHERE_YELLOW:
                ThunderTime = 100f;
                break;
        }
    }


    //同步仅仅调用该函数
    public void PlaySkillEffect(int type)
    {
        switch (type)
        {
            case (int)SphereType.SPHERE_RED:
                //ChaosTime = 100f;
                break;
            case (int)SphereType.SPHERE_BLUE:
                //FreezeTime = 100f;
                break;
            case (int)SphereType.SPHERE_YELLOW:
                //DisarmTime = 100f;
                break;
        }
    }
}
