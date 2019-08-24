using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float Speed  = 3f;
    private Rigidbody2D LocalPlayerRididbody;
    private NetWorkManager NetClass;
    private float MoveTime = 0f;
    private Vector3 CacheVec;
    private Vector2 direct;
    private bool _isDead = false;



    public Rigidbody2D PlayerRidibody
    {
        get{ return LocalPlayerRididbody; }
        set{ LocalPlayerRididbody = value; }
    }

    public bool isDead
    {
        get { return _isDead; }
        set { _isDead = value; }
    }

    private void FixedUpdate()
    {
        if (_isDead)
        {
            Vector2 currentVelocity = Vector2.zero;
            Vector2 position = transform.position;
            CacheVec = Vector2.SmoothDamp(transform.position, position + direct * Time.fixedDeltaTime * Speed, ref currentVelocity, MoveTime);
            CacheVec.z = -10f;
            transform.position = CacheVec;
        }
        else if (LocalPlayerRididbody != null)
        {
            Vector2 currentVelocity = Vector2.zero;
            CacheVec = Vector2.SmoothDamp(transform.position, LocalPlayerRididbody.position, ref currentVelocity, MoveTime);
            CacheVec.z = -10f;
            transform.position = CacheVec;
        }
    }

    private void LateUpdate()
    {
        direct.x = ETCInput.GetAxis("Horizontal");
        direct.y = ETCInput.GetAxis("Vertical");
    }
}
