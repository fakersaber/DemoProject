using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Rigidbody2D LocalPlayerRididbody;
    private NetWorkManager NetClass;
    private float MoveTime = 0.1f;
    private Vector2 currentVelocity = Vector2.zero;
    private Vector3 CacheVec;

    //private void Awake()
    //{
    //    //NetClass = GameObject.Find("GameManager").GetComponent<NetWorkManager>();
    //    //LocalPlayerRididbody = NetClass.AllPlayerInfo[NetClass.LocalPlayer].GetComponent<Rigidbody2D>();
    //}



    public Rigidbody2D PlayerRidibody
    {
        get{return LocalPlayerRididbody;}
        set{LocalPlayerRididbody = value;}
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (LocalPlayerRididbody != null)
        {
            CacheVec = Vector2.SmoothDamp(transform.position, LocalPlayerRididbody.position, ref currentVelocity, MoveTime);
            CacheVec.z = -10f;
            transform.position = CacheVec;
        }
    }
}
