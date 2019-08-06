using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerController : MonoBehaviour
{
    public float ReflectTime = 0.5f; //反弹变化时间
    public float InputTime = 0.5f; //输入变化时间

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

    public InputController InputTouch;
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


    private void FixedUpdate()
    {
        DeltaRotate = PlayerRigidbody.rotation - LastRotation;
        LastRotation = PlayerRigidbody.rotation;

        if (ReflectCurScale < 1f)
        {
            PlayerRigidbody.MovePosition(Vector2.Lerp(ReflectStartPosition, ReflectEndPosition, ReflectCurScale));
            PlayerRigidbody.MoveRotation(Mathf.LerpAngle(ReflectStartRotation, ReflectEndRotation, ReflectCurScale));
            ReflectCurScale += ReflectLerpScaleDelta;
        }
        else
        {
            if (InputTouch.direct.sqrMagnitude > 1e-7)
            {
                PlayerMove();
                PlayerRotation();
            }
        }
    }




    private void PlayerMove()
    {
        //开启加速改变速度，插入墙不改变
        Vector2 DeltaPostion = InputTouch.direct * Time.fixedDeltaTime * 3f;
        PlayerRigidbody.MovePosition(PlayerRigidbody.position + DeltaPostion);

    }


    private void PlayerRotation()
    {
        //开启加速，输入为空，插入墙上均不改变转向
        if (InputTouch.direct.sqrMagnitude > 1e-7)
        {
            //输入改变更新,dir同方向也不行，所以只能映射角度
            EndInputRotation = MappingRotation(InputTouch.direct.normalized);
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

    private float MappingRotation(Vector2 dir)
    {
        //sina = sin(PI/2 - b) = cosb  刚好变换为以竖直方向为0的角度
        float sita = Mathf.Acos(dir.normalized.y) * 180f / Mathf.PI;
        if (Vector2.Dot(dir, Vector2.right) > 0f)
            sita = -sita;   //一四象限为负,与引擎角度相同
        return sita;
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
    }



}


