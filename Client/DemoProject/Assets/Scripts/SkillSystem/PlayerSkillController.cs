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
    public float DisarmTime = 0f; //缴械时间



    private void Awake()
    {
        GameObject GameManager = GameObject.FindWithTag("GameManager");
        NetClass = GameManager.GetComponent<NetWorkManager>();
        PlayerRigidbody = GetComponent<Rigidbody2D>();
    }



    //该函数仅仅被EnergyController调用
    public void ReleaseSkill(int type)
    {
        Debug.Log("Skill: " + type);
        UpdateSkillInfo.Type = type;
        UpdateVec.X = PlayerRigidbody.position.x;
        UpdateVec.Y = PlayerRigidbody.position.y;
        NetClass.SendDataToServer(UpdateSkillInfo,(int)Protocal.MESSAGE_RELEASESKILL);
        PlaySkillEffect(type);
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
                DisarmTime = 100f;
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
