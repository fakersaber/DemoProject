using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    ////显示进度的文本
    //private Text progress;
    ////进度条的数值
    //private float progressValue;
    ////进度条
    //private Slider slider;
    ////[Tooltip("下个场景的名字")]
    ////public string nextSceneName;

    private AsyncOperation async = null;
    private SpriteRenderer LoadingImage;
    private Button StartGame;
    private void Awake()
    {
        LoadingImage = GetComponent<SpriteRenderer>();
    }

    //IEnumerator LoadScene()
    //{
    //    //async = SceneManager.LoadSceneAsync(nextSceneName);
    //    async.allowSceneActivation = false;
    //    while (!async.isDone)
    //    {
    //        if (async.progress < 0.9f)
    //            progressValue = async.progress;
    //        else
    //            progressValue = 1.0f;

    //        slider.value = progressValue;
    //        progress.text = (int)(slider.value * 100) + " %";

    //        if (progressValue >= 0.9)
    //        {
    //            progress.text = "按任意键继续";
    //            if (Input.anyKeyDown)
    //            {
    //                async.allowSceneActivation = true;
    //            }
    //        }
    //        yield return null;
    //    }
    //}

    public void ToGameScene()
    {
        LoadingImage.enabled = true;
        GameObject.FindWithTag("StartGameButton").GetComponent<Button>().interactable = false;
        SceneManager.LoadSceneAsync("SampleScene");
    }

    public void ToUIScene()
    {
        AudioController.Play("Effect");
        SceneManager.LoadScene("UI");
    }
}
