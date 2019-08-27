﻿using UnityEngine;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    public const int RoomSize = 4;
    public int SlowFrame = 0;

    private int _LocalDownPlayerNum = 0;
    private int _OtherDownPlayerNum = 0;
    private int _AliveSize = RoomSize;
    private GameObject MainCamera;
    private NetWorkManager NetClass;
    private SpriteRenderer LoadingImage;
    private Material LoadMaterial;
    private CanvasGroup EndSetting;
    private CanvasGroup ControllerSetting;
    private DG.Tweening.DOTweenAnimation ScenesController;
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


    public int AliveSize
    {
        get { return _AliveSize; }
        set { _AliveSize = value; }
    }


    private void Awake()
    {
        ControllerSetting = GameObject.Find("EasyTouchControlsCanvas").GetComponent<CanvasGroup>();
        MainCamera = GameObject.FindWithTag("MainCamera");
        NetClass = GetComponent<NetWorkManager>();
        LoadingImage = GetComponent<SpriteRenderer>();
        LoadMaterial = LoadingImage.material;
        LoadMaterial.SetFloat("_Radius", Radius);
        EndSetting = GameObject.Find("Canvas_End").GetComponent<CanvasGroup>();
        ScenesController = GameObject.FindWithTag("CanvasPanel").GetComponent<DG.Tweening.DOTweenAnimation>();
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

                ControllerSetting.alpha = 1f;
                ControllerSetting.interactable = true;
                ControllerSetting.blocksRaycasts = true;
                LoadingImage.enabled = false;
                yield break;
            }
            yield return null;
        }
        
    }


    private void FixedUpdate()
    {
        if (SlowFrame > 0)
        {
            SlowFrame--;
            Time.timeScale = 0.4f;
            if (SlowFrame == 0)
            {
                EndSetting.alpha = 1f;
                EndSetting.interactable = true;
                EndSetting.blocksRaycasts = true;

                ControllerSetting.alpha = 0f;
                ControllerSetting.interactable = false;
                ControllerSetting.blocksRaycasts = false;
                ScenesController.DOPlay();
            }
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}
