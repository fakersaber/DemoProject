using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioInUI : MonoBehaviour
{

    private void Awake()
    {
        AudioController.PlayMusic("BGM0");
    }

    public void StartBtn()
    {
        AudioController.Play("Effect0");
    }

}
