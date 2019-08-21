using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Rigidbody2D LocalPlayerRididbody;
    private NetWorkManager NetClass;
    private float MoveTime = 0f;
    private Vector3 CacheVec;


    public Rigidbody2D PlayerRidibody
    {
        get{ return LocalPlayerRididbody; }
        set{ LocalPlayerRididbody = value; }
    }


    private void FixedUpdate()
    {
        if (LocalPlayerRididbody != null)
        {
            Vector2 currentVelocity = Vector2.zero;
            CacheVec = Vector2.SmoothDamp(transform.position, LocalPlayerRididbody.position, ref currentVelocity, MoveTime);
            CacheVec.z = -10f;
            transform.position = CacheVec;
        }
    }
}
