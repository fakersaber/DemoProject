using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    //用异步？

    public void ToGameScene()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void ToUIScene()
    {
        SceneManager.LoadScene("UI");
    }
}
