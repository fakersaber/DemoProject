using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTest : MonoBehaviour
{
   public void PlayEffect1()
    {
        AudioController.Play("Effect");
    }

    public void PlayEffect2()
    {
        AudioController.Play("Effect1");
    }

    private void Start()
    {
        AudioController.PlayMusic("bgm1");

    }

    public void PlayBGM1()
    {
        AudioController.PlayMusic("bgm1");
    }

    public void StopBGM1()
    {
        AudioController.StopMusic();
    }

   
}
