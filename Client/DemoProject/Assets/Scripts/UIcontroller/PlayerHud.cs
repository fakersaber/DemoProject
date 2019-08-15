using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHud : MonoBehaviour
{
    //Stack<GameObject> gameObjects;
    private Hud hud;
    private float val;
    private float damage;
    private Sprite[] energySp;
    private Transform[] energyHuds;
    private int count;

    private void Awake()
    {
        energySp = Resources.LoadAll<Sprite>("heart");
        energyHuds = GameObject.Find("Energy Hud").GetComponentsInChildren<Transform>();
        hud = new Hud(1f);
        val = 1f;
        damage = 0.2f;
        count = 1;
    }

    public void Init()
    {
        InitHPHud();
        InitEnergyHud();
    }

    public void InitHPHud()
    {
        //初始化血条和能量槽
        val = 1f;
        hud.UpdateHud(val);    
    }

    public void InitEnergyHud()
    {
        for (int i = 1; i < 4; i++)
        {
            energyHuds[i].GetComponentInChildren<SpriteRenderer>().sprite = energySp[1];  
        }
    }

    public void HPDecrese()
    {
        val -= damage / 1f;
        hud.UpdateHud(val);
    }

    //使用技能-3
    public void EnergySlot()
    {
        if (count != 4) return;
        InitEnergyHud();
    }

    //获得能量+1
    public void GetEnergy()
    {
        if (count >= 4) { return; }

        count += 1;
        energyHuds[count - 1].GetComponentInChildren<SpriteRenderer>().sprite = energySp[2];
    }

    //使用能量-1
    public void UseEnergy()
    {
        if (count <= 1) { return; }
        count -= 1;
        energyHuds[count].GetComponentInChildren<SpriteRenderer>().sprite = energySp[1];
    }
}