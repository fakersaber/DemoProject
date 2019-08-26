using UnityEngine;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    private int _LocalDownPlayerNum = 0;
    private int _OtherDownPlayerNum = 0;
    public const int RoomSize = 4;
    private GameObject AllUI;
    private GameObject MainCamera;
    private NetWorkManager NetClass;
    private SpriteRenderer LoadingImage;
    private Material LoadMaterial;

    private float Radius = 0.707f;

    public int LocalDownPlayer
    {
        get { return _LocalDownPlayerNum; }
        set { _LocalDownPlayerNum = value; }
    }


    public int OtherDownPlayerNum
    {
        get { return _OtherDownPlayerNum; }
        set { _OtherDownPlayerNum = value; }
    }

    private void Awake()
    {
        AllUI = GameObject.Find("EasyTouchControlsCanvas");
        MainCamera = GameObject.FindWithTag("MainCamera");
        NetClass = GetComponent<NetWorkManager>();
        LoadingImage = GetComponent<SpriteRenderer>();
        LoadMaterial = LoadingImage.material;
        LoadMaterial.SetFloat("_Radius", Radius);
    }

    private void Start()
    {
        StartCoroutine(CheckStart());
    }

    public IEnumerator CheckStart()
    {
        while (true)
        {
            if(_LocalDownPlayerNum == RoomSize && _OtherDownPlayerNum == RoomSize - 1)
            {
                while (Radius > 0f)
                {
                    Radius -= 0.02f;
                    LoadMaterial.SetFloat("_Radius", Radius);
                    yield return null;
                }
                MainCamera.GetComponent<CameraController>().PlayerRidibody = NetClass.AllPlayerRigidy[NetClass.LocalPlayer];


                LoadingImage.enabled = false;
                var Setting = AllUI.GetComponent<CanvasGroup>();
                Setting.alpha = 1f;
                Setting.interactable = true;
                Setting.blocksRaycasts = true;
                LoadingImage.enabled = false;
                yield break;
            }
            yield return null;
        }
        
    }
}
