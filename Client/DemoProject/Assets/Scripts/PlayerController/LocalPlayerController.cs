using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LocalPlayerController : MonoBehaviour
{

    #region 
    public float ReflectTime = 0.35f; //反弹变化时间
    public float InputTime = 0.15f; //输入变化时间
    public float NormalSpeed = 6.5f; //正常速度
    public float SpurtSpeed = 15f; //冲刺速度
    public float ReflectScale = 1.5f; //反弹系数
    public float DeltaAngle = 30f;
    #endregion

    #region
    public float ReflectLerpScaleDelta = 0f;
    public float ReflectCurScale = 2f; //设置为2防止初始进入插值情况，反弹插值系数
    public Vector2 ReflectStartPosition;
    public Vector2 ReflectEndPosition;
    public float ReflectStartRotation;
    public float ReflectEndRotation;
    public float LastRotation;
    public float DeltaRotate = 0f;
    #endregion

    #region
    private float InputLerpScaleDelta = 0f;
    private float InputCurScale = 2f; //输入插值系数
    private float StartInputRotation;
    private float EndInputRotation;
    private float LastInputRotation;
    #endregion


    private Rigidbody2D PlayerRigidbody;
    private PlayerHealth Health;
    private PlayerSkillController SkillController;
    private PlayerEnergyController EnergyController;
    private NetWorkManager NetClass;
    private PlayerEffectsManager EffectsManager;
    private Animator animator;
    private Vector2 direct;
    private int CurWaitFrame = 0;
    SpurtButton SpurtTouch;


    private UpdateInfo UpdateClass = new UpdateInfo();
    private YVector2 UpdateVec = new YVector2();
    private AttakeInfo AttackClass = new AttakeInfo();

    private bool isOK = true; // 初始可以动
    private bool isSpurt = false;
    private bool isFreeze = false;
    private bool isChaos = false;
    private bool isThunder = false;

    #region
    private int WallLayer;
    private int PlayerLayer;
    #endregion


    private Transform HealthBarTrans;
    private Transform EnergyRenderTrans;
    private Transform FakeCenter;
    private Vector3 OffsetHealth;
    private Vector3 OffsetEnergy;

    private void Awake()
    {
        PlayerRigidbody = GetComponent<Rigidbody2D>();
        Health = GetComponent<PlayerHealth>();
        ReflectLerpScaleDelta = Time.fixedDeltaTime / ReflectTime;
        InputLerpScaleDelta = Time.fixedDeltaTime / InputTime;
        var GameManager = GameObject.FindWithTag("GameManager");
        NetClass = GameManager.GetComponent<NetWorkManager>();
        EffectsManager = GameManager.GetComponent<PlayerEffectsManager>();
        SpurtTouch = GameObject.FindWithTag("Spurt").GetComponent<SpurtButton>();
        SkillController = GetComponent<PlayerSkillController>();
        EnergyController = GetComponent<PlayerEnergyController>();
        WallLayer = LayerMask.NameToLayer("Wall");
        PlayerLayer = LayerMask.NameToLayer("Player");
        HealthBarTrans = GetComponentsInChildren<Transform>()[5];
        FakeCenter = GetComponentsInChildren<Transform>()[6];
        EnergyRenderTrans = GetComponentsInChildren<Transform>()[7];
        animator = GetComponent<Animator>();
    }


    #region
    private void CheckStatus()
    {
        if (SpurtTouch.SpurtTime > 0f)
        {
            SpurtTouch.SpurtTime -= Time.fixedDeltaTime;
            isSpurt = true;
        }
        else
        {
            isSpurt = false;
        }

        if (SkillController.ChaosTime > 0f)
        {
            SkillController.ChaosTime -= Time.fixedDeltaTime;
            isChaos = true;
        }
        else
        {
            if (SkillController.ChaosEffect.isPlaying)
                SkillController.ChaosEffect.Stop();
            isChaos = false;
        }

        if (SkillController.FreezeTime > 0f)
        {
            SkillController.FreezeTime -= Time.fixedDeltaTime;
            isFreeze = true;
        }
        else
        {
            animator.SetBool("iced", false);
            isFreeze = false;
        }

        if (SkillController.ThunderTime > 0f)
        {
            SkillController.ThunderTime -= Time.fixedDeltaTime;
            isThunder = true;
        }
        else
        {
            if (SkillController.ThunderEffect.isPlaying)
                SkillController.ThunderEffect.Stop();
            isThunder = false;
        }

        if (SkillController.SuperTime > 0f)
        {
            SkillController.SuperTime -= Time.fixedDeltaTime;
            //Debug.Log(SkillController.SuperTime);
            if(NetClass.LocalPlayer == 1 && SkillController.SuperTime < 0f )
            {
                for (int i = 0; i < 3; ++i)
                {
                    EnergyController.ConsumeEnergySphere();
                }
            }
        }
        else
        {
            if (SkillController.SelfEffectChaos.isPlaying)
                SkillController.SelfEffectChaos.Stop();
            if (SkillController.SelfEffectIce.isPlaying)
                SkillController.SelfEffectIce.Stop();
        }
    }
    #endregion



    private void Start()
    {
        StartInputRotation = PlayerRigidbody.rotation;
        EndInputRotation = PlayerRigidbody.rotation;
        LastInputRotation = PlayerRigidbody.rotation;
        LastRotation = PlayerRigidbody.rotation;
        OffsetHealth = HealthBarTrans.position - FakeCenter.position;
        OffsetEnergy = EnergyRenderTrans.position - FakeCenter.position;
    }

    private void Update()
    {
        HealthBarTrans.position = OffsetHealth + FakeCenter.transform.position;
        HealthBarTrans.rotation = Quaternion.Euler(0f, 0f, 0f);
        EnergyRenderTrans.position = OffsetEnergy + FakeCenter.transform.position;
        EnergyRenderTrans.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    private void LateUpdate()
    {
        direct.x = ETCInput.GetAxis("Horizontal");
        direct.y = ETCInput.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        UpdateCode();
        CheckStatus();

        //反弹插值
        if (ReflectCurScale <= 1f)
        {
            ReflectCurScale += ReflectLerpScaleDelta;
            PlayerRigidbody.MovePosition(Vector2.Lerp(ReflectStartPosition, ReflectEndPosition, ReflectCurScale));
            PlayerRigidbody.MoveRotation(Mathf.LerpAngle(ReflectStartRotation, ReflectEndRotation, ReflectCurScale));

            if(ReflectCurScale > 1f)
            {
                isOK = true;
                CurWaitFrame = 0;
                InputCurScale = 2f;
            }
        }
        else
        {
            //等待帧
            if (!isOK)
            {
                CurWaitFrame++;
                if (CurWaitFrame < 2)
                    return;
                CurWaitFrame = 0;
                isOK = true;
            }




            #region
            if (isSpurt)
            {
                Vector2 DeltaPostion;
                DeltaPostion = PlayerRigidbody.transform.up * Time.fixedDeltaTime * SpurtSpeed;
                PlayerRigidbody.MovePosition(PlayerRigidbody.position + DeltaPostion);
                SendData(NetClass.LocalPlayer, PlayerRigidbody.position + DeltaPostion, PlayerRigidbody.rotation, (int)Protocal.MESSAGE_UPDATEDATA);
                return;
            }
            #endregion

            #region
            if (isFreeze)
                return;
            #endregion

            if (direct.sqrMagnitude > 1e-7)
            {
                Vector2 DeltaPostion;

                #region
                if (isChaos)
                {
                    direct.x = -direct.x;
                    direct.y = -direct.y;
                }
                #endregion

                DeltaPostion = direct * Time.fixedDeltaTime * NormalSpeed;
                PlayerRigidbody.MovePosition(PlayerRigidbody.position + DeltaPostion);
                EndInputRotation = MathTool.MappingRotation(direct.normalized);
                //如果输入发生变化或者是当前是发生反弹后，从当前位置重新插值
                if (Mathf.Abs(LastInputRotation - EndInputRotation) > 1e-6 || Mathf.Abs(InputCurScale - 2.0f) < 1e-6)
                {
                    StartInputRotation = PlayerRigidbody.rotation;
                    LastInputRotation = EndInputRotation;
                    InputCurScale = 0f;
                }
                if (InputCurScale <= 1f)
                {
                    InputCurScale += InputLerpScaleDelta;
                }
                float CurAng = Mathf.LerpAngle(StartInputRotation, EndInputRotation, InputCurScale);
                PlayerRigidbody.MoveRotation(CurAng);
                SendData(NetClass.LocalPlayer, PlayerRigidbody.position + DeltaPostion, CurAng,(int)Protocal.MESSAGE_UPDATEDATA);

            }
        }
    }


    private void SendData(int id, Vector2 position, float rotation,int protocal)
    {
        UpdateClass.PlayerId = id;
        UpdateVec.X = position.x;
        UpdateVec.Y = position.y;
        UpdateClass.Position = UpdateVec;
        UpdateClass.Rotation = rotation;
        NetClass.SendDataToServer(UpdateClass, protocal);
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(NetClass.LocalPlayer == 1)
        {
            Vector2 VelocityDir = Vector2.zero;
            if (collision.gameObject.layer == PlayerLayer)
            {
                int SelfIndex = collision.otherCollider.GetHashCode();
                int otherIndex = collision.collider.GetHashCode();
                PlayerHealth otherHealth = collision.gameObject.GetComponent<PlayerHealth>();
                for (int i = 0; i < collision.contactCount; ++i)
                    VelocityDir += (collision.otherRigidbody.worldCenterOfMass - collision.contacts[i].point).normalized;
                if (Health.WeaponIndex == SelfIndex && otherHealth.WeaponIndex == otherIndex)
                {
                    SendAttackInfo((int)SpecialEffects.WEAPONTOWEAPON, 0, collision.contacts[0].point);
                    AudioController.Play("Effect5");
                    EffectsManager.PlayerSpecialEffects(NetClass.LocalPlayer,(int)SpecialEffects.WEAPONTOWEAPON, collision.contacts[0].point);
                }
                else if (Health.BodyIndex == SelfIndex && otherHealth.BodyIndex == otherIndex)
                {
                    SendAttackInfo((int)SpecialEffects.BADYTOBADY, 0, collision.contacts[0].point);
                    AudioController.Play("Effect4");
                    EffectsManager.PlayerSpecialEffects(NetClass.LocalPlayer,(int)SpecialEffects.BADYTOBADY, collision.contacts[0].point);
                }
                else if (Health.BodyIndex == SelfIndex && otherHealth.WeaponIndex == otherIndex)
                {
                    int CurDamage = PlayerHealth.NormalDamage;
                    if (collision.gameObject.GetComponent<PlayerSkillController>().ThunderTime > 0f)
                        CurDamage = PlayerHealth.ThunderDamage;
                    SendAttackInfo((int)SpecialEffects.BADYTOWEAPON, CurDamage, collision.contacts[0].point);
                    AudioController.Play("Effect4");
                    EffectsManager.PlayerSpecialEffects(NetClass.LocalPlayer, (int)SpecialEffects.BADYTOWEAPON, collision.contacts[0].point);
                    Health.SubHp(CurDamage);
                }
                else if (Health.HeadIndex == SelfIndex && otherHealth.WeaponIndex == otherIndex)
                {
                    int CurDamage = PlayerHealth.NormalDamage * PlayerHealth.CriticalDamage;
                    if (collision.gameObject.GetComponent<PlayerSkillController>().ThunderTime > 0f)
                        CurDamage = PlayerHealth.ThunderDamage * PlayerHealth.CriticalDamage;
                    SendAttackInfo((int)SpecialEffects.BADYTOWEAPON, CurDamage, collision.contacts[0].point);
                    AudioController.Play("Effect4");
                    EffectsManager.PlayerSpecialEffects(NetClass.LocalPlayer, (int)SpecialEffects.BADYTOWEAPON, collision.contacts[0].point);
                    Health.SubHp(CurDamage);
                }
            }
            else if(collision.gameObject.layer == WallLayer)
            {
                for (int i = 0; i < collision.contactCount; ++i)
                    VelocityDir += (collision.otherRigidbody.worldCenterOfMass - collision.contacts[i].point).normalized;
            }

            ReflectStartPosition = PlayerRigidbody.position;
            ReflectEndPosition = PlayerRigidbody.position + VelocityDir.normalized * ReflectScale;
            ReflectStartRotation = PlayerRigidbody.rotation;
            ReflectEndRotation = PlayerRigidbody.rotation + (Mathf.Abs(DeltaRotate) < 1e-6 ? 0f : (DeltaRotate < 0f ? DeltaAngle : -DeltaAngle));
            ReflectCurScale = 0f;
            SendData(NetClass.LocalPlayer, ReflectEndPosition, ReflectEndRotation, (int)Protocal.MESSAGE_REFLECTDATA);
        }
        else
        {
            //等待反弹同步信息到来之前不能进行任何操作
            isOK = false;
        }
    }


    private void SendAttackInfo(int EffectsIndex, int Damage, Vector2 ContactPoint)
    {
        AttackClass.PlayerId = NetClass.LocalPlayer;
        UpdateVec.X = ContactPoint.x;
        UpdateVec.Y = ContactPoint.y;
        AttackClass.Position = UpdateVec;
        AttackClass.EffectsIndex = EffectsIndex;
        AttackClass.Damage = Damage;
        NetClass.SendDataToServer(AttackClass, (int)Protocal.MESSAGE_DAMAGE);
    }


    private void UpdateCode()
    {
        PlayerRigidbody.angularVelocity = 0f;
        PlayerRigidbody.velocity = Vector2.zero;
        if (NetClass.LocalPlayer == 1)
        {
            DeltaRotate = PlayerRigidbody.rotation - LastRotation;
            LastRotation = PlayerRigidbody.rotation;
        }
    }
}


