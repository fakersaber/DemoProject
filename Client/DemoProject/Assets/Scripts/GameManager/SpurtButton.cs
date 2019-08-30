using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpurtButton : MonoBehaviour
{
    private float _SpurtTime = 0f;
    private PlayerEnergyController _LocalPlayerEnergy;
    private NetWorkManager NetClass;

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

    private void Awake()
    {
        GameObject GameManager = GameObject.FindWithTag("GameManager");
        NetClass = GameManager.GetComponent<NetWorkManager>();
    }


    public void SetSpurtTime()
    {
        if (_SpurtTime <=0f && _LocalPlayerEnergy.ConsumeEnergySphere())
        {
            _SpurtTime = 0.35f;

            AudioController.Play("Effect0");
        }
    }



}
