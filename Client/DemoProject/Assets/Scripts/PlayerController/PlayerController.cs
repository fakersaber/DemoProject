using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerController : MonoBehaviour
{
    public float ReflectTime = 0.5f; //反弹变化时间

    private NetWorkManager NetClass;
    private Rigidbody2D PlayerRigidbody;
    private PlayerHealth Health;
    private bool isOK = true;
    private int CurWaitFrame = 0;

    public Vector2 StartSynchronizepos;
    public Vector2 EndSynchronizepos;
    public float StartSynchronizerot;
    public float EndSynchronizerot;


    public float ReflectLerpScaleDelta = 0f;
    public Vector2 ReflectStartPosition;
    public Vector2 ReflectEndPosition;
    public float ReflectStartRotation;
    public float ReflectEndRotation;
    public float LastRotation;
    public float DeltaRotate;

    private Vector2 TargetPosition;
    public float ReflectCurScale = 2f; //设置为2防止初始进入插值情况，反弹插值系数
    public float NetCurScale = 2f;//输入旋转系数
    public float NetPositionScale = 2f;
    public int PlayerId;

    //用于同步的缓存类
    private UpdateInfo UpdateClass = new UpdateInfo();
    private YVector2 UpdateVec = new YVector2();
    private AttakeInfo AttackClass = new AttakeInfo();

    #region
    private int WallLayer;
    private int PlayerLayer;
    #endregion

    private void Awake()
    {
        PlayerRigidbody = GetComponent<Rigidbody2D>();
        Health = GetComponent<PlayerHealth>();
        NetClass = GameObject.FindWithTag("GameManager").GetComponent<NetWorkManager>();
        ReflectLerpScaleDelta = Time.fixedDeltaTime / ReflectTime;
        WallLayer = LayerMask.NameToLayer("Wall");
        PlayerLayer = LayerMask.NameToLayer("Player");
    }

    private void Start()
    {
        LastRotation = PlayerRigidbody.rotation;
    }


    private void FixedUpdate()
    {
        UpdateCode();
        //同步反弹插值
        if (ReflectCurScale <= 1f)
        {
            ReflectCurScale += ReflectLerpScaleDelta;
            PlayerRigidbody.MovePosition(Vector2.Lerp(ReflectStartPosition, ReflectEndPosition, ReflectCurScale));
            PlayerRigidbody.MoveRotation(Mathf.LerpAngle(ReflectStartRotation, ReflectEndRotation, ReflectCurScale));

            if (ReflectCurScale > 1f)
            {
                isOK = true;
                CurWaitFrame = 0;
                NetCurScale = 2f;
                NetPositionScale = 2f;
            }
        }

        //同步移动插值
        else
        {
            if (!isOK)
            {
                CurWaitFrame++;
                if (CurWaitFrame < 3) return;
                CurWaitFrame = 0;
                isOK = true;
                //return;
            }

            if (NetCurScale <= 1f)
            {
                NetCurScale += 0.2f;
                PlayerRigidbody.MoveRotation(Mathf.LerpAngle(StartSynchronizerot, EndSynchronizerot, NetCurScale));
            }
            if (NetPositionScale <= 1f)
            {
                NetPositionScale += 0.5f;
                PlayerRigidbody.MovePosition(Vector2.Lerp(StartSynchronizepos, EndSynchronizepos, NetPositionScale));
            }
        }
    }




    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (NetClass.LocalPlayer == 1)
        {
            Vector2 VelocityDir = Vector2.zero;
            if (collision.gameObject.layer == PlayerLayer)
            {
                int SelfIndex = collision.otherCollider.GetHashCode();
                int otherIndex = collision.collider.GetHashCode();
                PlayerHealth otherHealth = collision.gameObject.GetComponent<PlayerHealth>();

                if (Health.WeaponIndex == SelfIndex && otherHealth.WeaponIndex == otherIndex)
                {
                    SendAttackInfo((int)SpecialEffects.WEAPONTOWEAPON, 0, collision.contacts[0].point);
                    Health.PlayerSpecialEffects((int)SpecialEffects.WEAPONTOWEAPON, collision.contacts[0].point);
                }
                else if (Health.BodyIndex == SelfIndex && otherHealth.BodyIndex == otherIndex)
                {
                    SendAttackInfo((int)SpecialEffects.BADYTOBADY, 0, collision.contacts[0].point);
                    Health.PlayerSpecialEffects((int)SpecialEffects.BADYTOBADY, collision.contacts[0].point);
                }
                else if (Health.BodyIndex == SelfIndex && otherHealth.WeaponIndex == otherIndex)
                {
                    SendAttackInfo((int)SpecialEffects.BADYTOWEAPON, 2, collision.contacts[0].point);
                    Health.PlayerSpecialEffects((int)SpecialEffects.BADYTOWEAPON, collision.contacts[0].point);
                    Health.SubHp(2);
                }
                for (int i = 0; i < collision.contactCount; ++i)
                    VelocityDir += (collision.contacts[i].point - collision.rigidbody.worldCenterOfMass).normalized;
            }
            else if (collision.gameObject.layer == WallLayer)
            {
                for (int i = 0; i < collision.contactCount; ++i)
                    VelocityDir += (collision.otherRigidbody.worldCenterOfMass - collision.contacts[i].point).normalized;
            }
            ReflectStartPosition = PlayerRigidbody.position;
            ReflectEndPosition = PlayerRigidbody.position + VelocityDir.normalized;
            ReflectStartRotation = PlayerRigidbody.rotation;
            ReflectEndRotation = PlayerRigidbody.rotation + (Mathf.Abs(DeltaRotate) < 1e-6 ? 0f : (DeltaRotate < 0f ? 45f : -45f));
            ReflectCurScale = 0f;

            UpdateClass.PlayerId = PlayerId;
            UpdateVec.X = ReflectEndPosition.x;
            UpdateVec.Y = ReflectEndPosition.y;
            UpdateClass.Position = UpdateVec;
            UpdateClass.Rotation = ReflectEndRotation;
            NetClass.SendDataToServer(UpdateClass, (int)Protocal.MESSAGE_REFLECTDATA);
        }
        else
        {
            //特效播放都是主端发送消息到本地，不是主端不管碰撞逻辑
            isOK = false;
        }
    }



    private void SendAttackInfo(int EffectsIndex,int Damage,Vector2 ContactPoint)
    {
        AttackClass.PlayerId = PlayerId;
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


