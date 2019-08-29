using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera camera;
    public float Speed  = 3f;
    private Rigidbody2D LocalPlayerRididbody;
    private NetWorkManager NetClass;
    private float MoveTime = 0f;
    private Vector3 CacheVec;
    private Vector2 direct;
    private bool _isDead = false;
    private float width_value = 0f;
    private float height_value = 0f;


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

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    private void Start()
    {
        width_value = 19.2f - camera.orthographicSize * camera.aspect;
        height_value = 10.8f - camera.orthographicSize;
    }



    private void FixedUpdate()
    {
        if (_isDead)
        {
            Vector2 currentVelocity = Vector2.zero;
            Vector2 position = transform.position;
            CacheVec = Vector2.SmoothDamp(transform.position, position + direct * Time.fixedDeltaTime * Speed, ref currentVelocity, MoveTime);
            CacheVec.x = Mathf.Clamp(CacheVec.x, -width_value, width_value);
            CacheVec.y = Mathf.Clamp(CacheVec.y,-height_value,height_value);
            CacheVec.z = -10f;
            transform.position = CacheVec;
        }
        else if (LocalPlayerRididbody != null)
        {
            Vector2 currentVelocity = Vector2.zero;
            CacheVec = Vector2.SmoothDamp(transform.position, LocalPlayerRididbody.position, ref currentVelocity, MoveTime);
            CacheVec.x = Mathf.Clamp(CacheVec.x, -width_value, width_value);
            CacheVec.y = Mathf.Clamp(CacheVec.y, -height_value, height_value);
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
