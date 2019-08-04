using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    public float AngleAcceleration = 45f;
    public float VeclocityAcceleration = 2f;

    private Rigidbody2D PlayerRigidbody;
    private float InputVertical = 0f;
    private float InputHorizon = 0f;

    public float MaxSpeed = 4f;
    public float MaxAngSpeed = 90f;
    public float ReflectSpeed = 3f;
    public float ReflectAngSpeed = 45f;

    //为true表示碰撞前为静止状态
    private float LastAngularVelocity = 0f;

    private void Awake()
    {
        PlayerRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        PlayerRigidbody.centerOfMass = new Vector2(0f, -0.6f);
        PlayerRigidbody.velocity = new Vector2(0f, 0f);
        PlayerRigidbody.angularVelocity = 0f;
    }

    private void Update()
    {
        InputVertical = Input.GetAxis("Vertical2");
        InputHorizon = Input.GetAxis("Horizontal2");
    }

    private void FixedUpdate()
    {
        //InputVertical * PlayerRigidbody.transform.up 为输入方向
        Vector2 DeltaVelocity = InputVertical * transform.up * VeclocityAcceleration * Time.fixedDeltaTime;
        Vector2 NowVelocity = PlayerRigidbody.velocity + DeltaVelocity;
        NowVelocity.x = Mathf.Clamp(NowVelocity.x, -MaxSpeed, MaxSpeed);
        NowVelocity.y = Mathf.Clamp(NowVelocity.y, -MaxSpeed, MaxSpeed);
        PlayerRigidbody.velocity = NowVelocity;



        float DeltaAngularVelocity = InputHorizon * AngleAcceleration * Time.fixedDeltaTime;
        float NowAngVelocity = PlayerRigidbody.angularVelocity + DeltaAngularVelocity;
        NowAngVelocity = Mathf.Clamp(NowAngVelocity, -MaxAngSpeed, MaxAngSpeed);
        PlayerRigidbody.angularVelocity = NowAngVelocity;

        LastAngularVelocity = PlayerRigidbody.angularVelocity;
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {

        #region
        /*
         * 碰撞时，两个基本原则
         * 1.当前刚体的角速度反向
         * 2.当前速度为对方的质心到碰撞点的向量   备选方案(法线)
        */

        Vector2 VelocityDir = (collision.contacts[0].point - collision.rigidbody.worldCenterOfMass).normalized;
        PlayerRigidbody.velocity = VelocityDir;

        //若刚体之前角速度为0，则根据VelocityDir与x轴的方向赋予角速度方向

        PlayerRigidbody.angularVelocity = Mathf.Abs(LastAngularVelocity) > 1e-6 ?
            (LastAngularVelocity < 0 ? 45f : -45f) :
            (Vector2.Dot(Vector2.right, VelocityDir) < 0 ? 45f : -45f);


        //PlayerRigidbody.angularVelocity = Vector2.Dot(Vector2.right, VelocityDir) < 0 ? 45f : -45f;

        //PlayerRigidbody.angularVelocity = 45f;
        #endregion
    }
}
