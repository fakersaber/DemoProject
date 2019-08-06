using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerController : MonoBehaviour
{
    //config
    public float ReflectTime = 0.5f; //反弹变化时间
    public float InputTime = 0.2f; //输入变化时间
    public float NormalSpeed = 3f; //正常速度
    public float SpurtSpeed = 6f; //冲刺速度
    public SpurtButton SpurtTouch;


    private float ReflectLerpScaleDelta = 0f;
    private float ReflectCurScale = 1f;
    private float InputLerpScaleDelta = 0f;
    private float InputCurScale = 1f;


    private Vector2 ReflectStartPosition;
    private Vector2 ReflectEndPosition;
    private float ReflectStartRotation;
    private float ReflectEndRotation;


    private float StartInputRotation;
    private float EndInputRotation;
    private float LastInputRotation;

    //用于反弹的上帧数据
    private float LastRotation;
    private float DeltaRotate;
    private Rigidbody2D PlayerRigidbody;


    private Vector2 direct;


    private bool isSpurt = false;

    private Vector2 ZeroVec;

    private void Awake()
    {
        PlayerRigidbody = GetComponent<Rigidbody2D>();
        ReflectLerpScaleDelta = Time.fixedDeltaTime / ReflectTime;
        InputLerpScaleDelta = Time.fixedDeltaTime / InputTime;
        ZeroVec = new Vector2(0f, 0f);
    }

    private void Start()
    {
        StartInputRotation = PlayerRigidbody.rotation;
        EndInputRotation = 0f;
        LastInputRotation = 0f;
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
        if (SpurtTouch.SpurtTime > 0f)
        {
            SpurtTouch.SpurtTime -= Time.fixedDeltaTime;
            isSpurt = true;
        }
        else
            isSpurt = false;


        if (ReflectCurScale < 1f)
        {
            PlayerRigidbody.MovePosition(Vector2.Lerp(ReflectStartPosition, ReflectEndPosition, ReflectCurScale));
            PlayerRigidbody.MoveRotation(Mathf.LerpAngle(ReflectStartRotation, ReflectEndRotation, ReflectCurScale));
            ReflectCurScale += ReflectLerpScaleDelta;
        }
        else
        {
            PlayerMove();
            PlayerRotation();
        }
    }



    private void PlayerMove()
    {
        Vector2 DeltaPostion;
        if (isSpurt)
            DeltaPostion = PlayerRigidbody.transform.up * Time.fixedDeltaTime * SpurtSpeed;
        else
            DeltaPostion = direct * Time.fixedDeltaTime * NormalSpeed;
        PlayerRigidbody.MovePosition(PlayerRigidbody.position + DeltaPostion);

    }


    private void PlayerRotation()
    {
        //开启加速，输入为空，插入墙上均不改变转向
        if (!isSpurt && direct.sqrMagnitude > 1e-7)
        {
            EndInputRotation = MathTool.MappingRotation(direct.normalized);
            //如果输入发生变化，插值起点发生改变
            if (Mathf.Abs(LastInputRotation - EndInputRotation) > 1e-6)
            {
                StartInputRotation = PlayerRigidbody.rotation;
                InputCurScale = InputLerpScaleDelta;
                LastInputRotation = EndInputRotation;
            }
            else if (InputCurScale < 1f)
            {
                InputCurScale += InputLerpScaleDelta;
            }
            PlayerRigidbody.MoveRotation(Mathf.LerpAngle(StartInputRotation, EndInputRotation, InputCurScale));
        }
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


