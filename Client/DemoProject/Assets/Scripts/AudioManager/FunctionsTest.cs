using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FunctionsTest : MonoBehaviour
{
    public void AudioTest1()
    {
        SceneManager.LoadScene("AudioTest1", LoadSceneMode.Single);
    }

    public void AudioTest2()
    {
        SceneManager.LoadScene("AudioTest2", LoadSceneMode.Single);
    }
}
