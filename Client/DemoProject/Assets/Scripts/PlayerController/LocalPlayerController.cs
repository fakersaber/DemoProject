using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LocalPlayerController : MonoBehaviour
{
    //config
    public float ReflectTime = 0.5f; //反弹变化时间
    public float InputTime = 0.2f; //输入变化时间
    public float NormalSpeed = 3f; //正常速度
    public float SpurtSpeed = 6f; //冲刺速度
    //public SpurtButton SpurtTouch;


    private float ReflectLerpScaleDelta = 0f;
    private float ReflectCurScale = 2f; //设置为2防止初始进入插值情况
    private float InputLerpScaleDelta = 0f;
    private float InputCurScale = 2f;


    private Vector2 ReflectStartPosition;
    private Vector2 ReflectEndPosition;
    private float ReflectStartRotation;
    private float ReflectEndRotation;


    private float StartInputRotation;
    private float EndInputRotation;
    private float LastInputRotation;
    private Vector2 TargetPosition;

    //用于反弹的上帧数据
    private float LastRotation;
    private float DeltaRotate;
    private Rigidbody2D PlayerRigidbody;
    private Vector2 direct;
    private bool isSpurt = false;
    private Vector2 ZeroVec;

   
    private NetWorkManager NetClass;

    //用于同步的缓存类
    private UpdateInfo UpdateClass;
    private YVector2 UpdateVec;


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
        TargetPosition = PlayerRigidbody.position;
        LastInputRotation =0f;
    }

    private void LateUpdate()
    {
        direct.x = ETCInput.GetAxis("Horizontal");
        direct.y = ETCInput.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        DeltaRotate = PlayerRigidbody.rotation - LastRotation;
        LastRotation = PlayerRigidbody.rotation;

        //检查冲刺时间
        //if (SpurtTouch.SpurtTime > 0f)
        //{
        //    SpurtTouch.SpurtTime -= Time.fixedDeltaTime;
        //    isSpurt = true;
        //}
        //else
        //    isSpurt = false;


        if (ReflectCurScale <= 1f)
        {
            PlayerRigidbody.MovePosition(Vector2.Lerp(ReflectStartPosition, ReflectEndPosition, ReflectCurScale));
            PlayerRigidbody.MoveRotation(Mathf.LerpAngle(ReflectStartRotation, ReflectEndRotation, ReflectCurScale));
            ReflectCurScale += ReflectLerpScaleDelta;
        }
        else
        {
            if(direct.sqrMagnitude > 1e-7)
            {
                Vector2 DeltaPostion;
                if (isSpurt)
                    DeltaPostion = PlayerRigidbody.transform.up * Time.fixedDeltaTime * SpurtSpeed;
                else
                    DeltaPostion = direct * Time.fixedDeltaTime * NormalSpeed;

                TargetPosition = PlayerRigidbody.position + DeltaPostion;
                if (!isSpurt)
                {
                    EndInputRotation = MathTool.MappingRotation(direct.normalized);
                    //如果输入发生变化，插值起点发生改变
                    if (Mathf.Abs(LastInputRotation - EndInputRotation) > 1e-6)
                    {
                        StartInputRotation = PlayerRigidbody.rotation;
                        LastInputRotation = EndInputRotation;
                        InputCurScale = InputLerpScaleDelta;
                    }
                }
                SendData(NetClass.LocalPlayer, TargetPosition, EndInputRotation);
            }

            if (InputCurScale <= 1f)
            {
                //即使没有输入也要确保客户端插值完毕，注意初始化全局值
                PlayerRigidbody.MovePosition(TargetPosition);
                PlayerRigidbody.MoveRotation(Mathf.LerpAngle(StartInputRotation, EndInputRotation, InputCurScale));
                InputCurScale += InputLerpScaleDelta;
            }
        }
    }


    private void SendData(int id,Vector2 position,float rotation)
    {
        UpdateClass.PlayerId = id;
        UpdateVec.X = position.x;
        UpdateVec.Y = position.y;
        UpdateClass.Position = UpdateVec;
        UpdateClass.Rotation = rotation;
        NetClass.AllPlayerInfo[NetClass.LocalPlayer] = UpdateClass;
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 VelocityDir = new Vector2(0f, 0f);
        for (int i = 0; i < collision.contactCount; ++i)
            VelocityDir += (collision.contacts[i].point - collision.rigidbody.worldCenterOfMass).normalized;
        ReflectStartPosition = PlayerRigidbody.position;
        ReflectEndPosition = PlayerRigidbody.position + VelocityDir.normalized;
        ReflectStartRotation = PlayerRigidbody.rotation;
        ReflectEndRotation = PlayerRigidbody.rotation + (Mathf.Abs(DeltaRotate) < 1e-6 ? 0f : (DeltaRotate < 0f ? 45f : -45f));
        ReflectCurScale = ReflectLerpScaleDelta;

        PlayerRigidbody.angularVelocity = 0f;
        PlayerRigidbody.velocity = ZeroVec;
    }

}


