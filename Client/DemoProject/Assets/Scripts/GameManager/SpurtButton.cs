using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpurtButton : MonoBehaviour
{
    private float _SpurtTime = 0f;
    private PlayerEnergyController _LocalPlayerEnergy;


    public float SpurtTime
    {
        get { return _SpurtTime; }
        set { _SpurtTime = value; }
    }


    public PlayerEnergyController LocalPlayerEnergy
    {
        get { return _LocalPlayerEnergy; }
        set { _LocalPlayerEnergy = value; }
    }


    public void SetSpurtTime()
    {
        if (_SpurtTime <=0f && LocalPlayerEnergy.ConsumeEnergySphere())
        {
            _SpurtTime = 0.35f;
            AudioController.Play("Effect0");
        }
    }



}
