using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LoadingManager : MonoBehaviour
{
    public const int RoomSize = 4;
    public int SlowFrame = 0;


    private int _LocalDownPlayerNum = 0;
    private int _OtherDownPlayerNum = 0;
    private int _AliveSize = RoomSize;
    private GameObject MainCamera;
    private NetWorkManager NetClass;
    private Image image;
    private Image[] imagePoints;
    // private SpriteRenderer LoadingImage;
    private Material LoadMaterial;
    private CanvasGroup EndSetting;
    private CanvasGroup ControllerSetting;
    //private GameObject EndScenes;
    private DG.Tweening.DOTweenAnimation ScenesController;

    #region //结算页面UI
    private Image[] EndScenesUI = new Image[16];
    private Sprite[] EndWordResource;
    private Sprite[] EndBackResource;
    private Sprite[] EndRankResource;
    private Sprite[] EndHeadResource;
    private Sprite[] HeroBackResource;
    private Sprite[] HeroResource;
    #endregion

    //本次游戏排名数据
    public List<int> RankList = new List<int>();



    private float Radius = 0.8f;

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
        //audioController = GameObject.Find("AudioController").GetComponent<AudioController>();
        MainCamera = GameObject.FindWithTag("MainCamera");
        NetClass = GetComponent<NetWorkManager>();
        image = GameObject.FindWithTag("LoadingImage").GetComponent<Image>();  
        LoadMaterial = image.material;
        LoadMaterial.SetFloat("_Radius", Radius);
        imagePoints = GameObject.FindWithTag("LoadingPoint").GetComponentsInChildren<Image>();
        for (int i=0;i<imagePoints.Length;i++)
        {
            imagePoints[i].material.SetFloat("_Radius", Radius);
        }
        //LoadingImage = GetComponent<SpriteRenderer>();
        //LoadMaterial = LoadingImage.material;

        EndSetting = GameObject.Find("Canvas_End").GetComponent<CanvasGroup>();
        //EndScenes = GameObject.FindWithTag("CanvasPanel");
        ScenesController = GameObject.FindWithTag("CanvasPanel").GetComponent<DG.Tweening.DOTweenAnimation>();
        var UIArray = ScenesController.GetComponentsInChildren<Image>();
        for (int i = 0; i < 16; ++i)
        {
            EndScenesUI[i] = UIArray[i + 1];
        }
        EndWordResource = Resources.LoadAll<Sprite>("EndWord");
        EndBackResource = Resources.LoadAll<Sprite>("EndBack");
        EndRankResource = Resources.LoadAll<Sprite>("EndRank");
        EndHeadResource = Resources.LoadAll<Sprite>("EndHead");
        HeroBackResource = Resources.LoadAll<Sprite>("HeroBack");
        HeroResource = Resources.LoadAll<Sprite>("Hero");
    }

    private void Start()
    {
        StartCoroutine(CheckStart());
    }

    public IEnumerator CheckStart()
    {
        AudioController.PlayMusic("BGM0");
        while (true)
        {
            if (_LocalDownPlayerNum == RoomSize && _OtherDownPlayerNum == RoomSize - 1)
            {
                while (Radius > 0f)
                {
                    Radius -= 0.04f;
                    image.material.SetFloat("_Radius", Radius);
                    for (int i = 0; i < imagePoints.Length; i++)
                    {
                        imagePoints[i].material.SetFloat("_Radius", Radius-0.3f);
                    }

                    yield return null;
                }
                MainCamera.GetComponent<CameraController>().PlayerRidibody = NetClass.AllPlayerRigidy[NetClass.LocalPlayer];

                ControllerSetting.alpha = 1f;
                ControllerSetting.interactable = true;
                ControllerSetting.blocksRaycasts = true;
                image.gameObject.SetActive(false);

                AudioController.PlayMusic("BGM1");
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
                AudioController.StopMusic();

                //最后的存活者
                if (NetClass.AllPlayerInfo[NetClass.LocalPlayer].activeSelf)
                    AudioController.Play("Effect3");
                else
                    AudioController.Play("Effect2");

                //设置对应结算页面
                SetEndScenes();

                ScenesController.DOPlay();
            }
        }
        else
        {
            Time.timeScale = 1f;
        }
    }


    private void SetEndScenes()
    {
        //hero类型与排名无关单独处理
        switch (NetClass.LocalPlayer)
        {
            case 1:
                EndScenesUI[14].sprite = HeroResource[1];
                break;
            case 2:
                EndScenesUI[14].sprite = HeroResource[2];
                break;
            case 3:
                EndScenesUI[14].sprite = HeroResource[0];
                break;
            case 4:
                EndScenesUI[14].sprite = HeroResource[3];
                break;
        }

        //i与排名的映射函数  Rank = -i + RoomSize
        for (int i = RoomSize - 1; i >= 0; i--)
        {
            //从当前名次的Player类型找到对应Resource中Sprite，对应Head位置均为-i + RoomSize + 2 * 4
            switch (RankList[i])
            {
                case 1:
                    EndScenesUI[-i + RoomSize + 2 * 4].sprite = EndHeadResource[3];
                    break;
                case 2:
                    EndScenesUI[-i + RoomSize + 2 * 4].sprite = EndHeadResource[0];
                    break;
                case 3:
                    EndScenesUI[-i + RoomSize + 2 * 4].sprite = EndHeadResource[1];
                    break;
                case 4:
                    EndScenesUI[-i + RoomSize + 2 * 4].sprite = EndHeadResource[2];
                    break;
            }


            //Rank映射，Rank是固定的可注释
            //EndScenesUI[-i + RoomSize + 1 * 4].sprite = EndRankResource[-i + RoomSize - 1];

            //自身处理back与hero_back的类型
            if (RankList[i] == NetClass.LocalPlayer)
            {
                if (i == RoomSize - 1)
                {
                    EndScenesUI[13].sprite = HeroBackResource[0];
                    EndScenesUI[15].sprite = HeroBackResource[1];
                }
                else
                {
                    EndScenesUI[13].sprite = HeroBackResource[2];
                    EndScenesUI[15].sprite = HeroBackResource[3];
                }

                EndScenesUI[-i + RoomSize].sprite = EndBackResource[0];
                //EndWord只跟排名有关
                EndScenesUI[0].sprite = EndWordResource[-i + RoomSize - 1];
                continue;
            }
            //Back映射
            EndScenesUI[-i + RoomSize + 0 * 4].sprite = EndBackResource[1];
        }

    }
}




//switch (i)  // y = -x + 4
//{
//    case 3:
//        //EndScenesUI[0].sprite = EndWordResource[0];
//        //EndScenesUI[5].sprite = EndRankResource[0];
//        //EndScenesUI[9].sprite = EndHeadResource[0];
//        EndScenesUI[1].sprite = EndBackResource[0];
//        EndScenesUI[13].sprite = HeroBackResource[0];  
//        EndScenesUI[15].sprite = HeroBackResource[1];  
//        break;
//    case 2:
//        EndScenesUI[2].sprite = EndBackResource[0];
//        EndScenesUI[13].sprite = EndBackResource[2];  //0 2
//        EndScenesUI[15].sprite = EndBackResource[3];  //1 3
//        break;
//    case 1:
//        EndScenesUI[3].sprite = EndBackResource[0];
//        EndScenesUI[13].sprite = EndBackResource[2];  //0 2
//        EndScenesUI[15].sprite = EndBackResource[3];  //1 3
//        break;
//    case 0:
//        EndScenesUI[4].sprite = EndBackResource[0];
//        EndScenesUI[13].sprite = EndBackResource[2];  //0 2
//        EndScenesUI[15].sprite = EndBackResource[3];  //1 3
//        break;
//}