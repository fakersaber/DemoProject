using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LocalPlayerController : MonoBehaviour
{

    public int TestCount = 0;

    //config
    public float ReflectTime = 0.5f; //反弹变化时间
    public float InputTime = 0.2f; //输入变化时间
    public float NormalSpeed = 3f; //正常速度
    public float SpurtSpeed = 6f; //冲刺速度

    public float ReflectLerpScaleDelta = 0f;
    public float ReflectCurScale = 2f; //设置为2防止初始进入插值情况，反弹插值系数
    public Vector2 ReflectStartPosition;
    public Vector2 ReflectEndPosition;
    public float ReflectStartRotation;
    public float ReflectEndRotation;
    public float LastRotation;
    public float DeltaRotate = 0f;


    private float InputLerpScaleDelta = 0f;
    private float InputCurScale = 2f; //输入插值系数
    private float StartInputRotation;
    private float EndInputRotation;
    private float LastInputRotation;



    private Rigidbody2D PlayerRigidbody;
    private Vector2 direct;
    private bool isSpurt = false;
    private Vector2 ZeroVec;


    private NetWorkManager NetClass;

    //用于同步的缓存类
    private UpdateInfo UpdateClass;
    private YVector2 UpdateVec;
    private bool isOK = true; // 初始可以动

    private int CurWaitFrame = 0;


    private void Awake()
    {
        PlayerRigidbody = GetComponent<Rigidbody2D>();
        ReflectLerpScaleDelta = Time.fixedDeltaTime / ReflectTime;
        InputLerpScaleDelta = Time.fixedDeltaTime / InputTime;
        ZeroVec = new Vector2(0f, 0f);
        NetClass = GameObject.Find("GameManager").GetComponent<NetWorkManager>();
        UpdateClass = new UpdateInfo();
        UpdateVec = new YVector2();
    }

    private void Start()
    {
        StartInputRotation = PlayerRigidbody.rotation;
        EndInputRotation = PlayerRigidbody.rotation;
        LastInputRotation = PlayerRigidbody.rotation;
        LastRotation = PlayerRigidbody.rotation;
    }

    private void LateUpdate()
    {
        direct.x = ETCInput.GetAxis("Horizontal");
        direct.y = ETCInput.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        UpdateCode();
        #region
        //检查冲刺时间
        //if (SpurtTouch.SpurtTime > 0f)
        //{
        //    SpurtTouch.SpurtTime -= Time.fixedDeltaTime;
        //    isSpurt = true;
        //}
        //else
        //    isSpurt = false;
        #endregion

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
        //移动插值
        else
        {
            if (!isOK)
            {
                CurWaitFrame++;
                if (CurWaitFrame < 3) return;
                CurWaitFrame = 0;
                isOK = true;
            }
            
            if (direct.sqrMagnitude > 1e-7)
            {
                Vector2 DeltaPostion;
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
            Debug.Log(collision.collider.GetHashCode());

            Vector2 VelocityDir = new Vector2(0f, 0f);
            for (int i = 0; i < collision.contactCount; ++i)
                VelocityDir += (collision.contacts[i].point - collision.rigidbody.worldCenterOfMass).normalized;
            ReflectStartPosition = PlayerRigidbody.position;
            ReflectEndPosition = PlayerRigidbody.position + VelocityDir.normalized;
            ReflectStartRotation = PlayerRigidbody.rotation;
            ReflectEndRotation = PlayerRigidbody.rotation + (Mathf.Abs(DeltaRotate) < 1e-6 ? 0f : (DeltaRotate < 0f ? 45f : -45f));
            ReflectCurScale = 0f;
            SendData(NetClass.LocalPlayer, ReflectEndPosition, ReflectEndRotation, (int)Protocal.MESSAGE_REFLECTDATA);
        }
        else
        {
            //等待反弹同步信息到来之前不能进行任何操作
            isOK = false;
        }

    }
    private void UpdateCode()
    {
        PlayerRigidbody.angularVelocity = 0f;
        PlayerRigidbody.velocity = ZeroVec;
        if (NetClass.LocalPlayer == 1)
        {
            DeltaRotate = PlayerRigidbody.rotation - LastRotation;
            LastRotation = PlayerRigidbody.rotation;
        }
    }
}


