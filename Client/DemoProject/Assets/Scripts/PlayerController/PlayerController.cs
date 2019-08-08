using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerController : MonoBehaviour
{

    NetWorkManager NetClass;
    Rigidbody2D PlayerRigidbody;
    private Vector2 ZeroVec;

    public Vector2 StartSynchronizepos;
    public Vector2 EndSynchronizepos;
    public float StartSynchronizerot;
    public float EndSynchronizerot;


    public float NetCurScale = 2f;


    private void Awake()
    {
        PlayerRigidbody = GetComponent<Rigidbody2D>();
        NetClass = GameObject.Find("GameManager").GetComponent<NetWorkManager>();
    }

    private void Start()
    {
        ZeroVec = new Vector2(0f, 0f);
        //StartSynchronizepos = PlayerRigidbody.position;
        //EndSynchronizepos = PlayerRigidbody.position;
    }


    private void FixedUpdate()
    {
        //若更新插值系数自动变化
        if (NetCurScale <= 1f)
        {
            //原客户端速度没有插值，
            PlayerRigidbody.MovePosition(Vector2.Lerp(StartSynchronizepos, EndSynchronizepos, NetCurScale));
            PlayerRigidbody.MoveRotation(Mathf.LerpAngle(StartSynchronizerot, EndSynchronizerot, NetCurScale));
            NetCurScale += 0.1f;
        }

    }




    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerRigidbody.angularVelocity = 0f;
        PlayerRigidbody.velocity = ZeroVec;
    }

}


