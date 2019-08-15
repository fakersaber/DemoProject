using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyUI : MonoBehaviour
{
    //渲染成一个条下，然后采样
    //记录一下索引，0，1，2
    //消耗球时索引-1，索引右边渲染成空球
    //增加球时渲染一个单位的对应贴图，

    //即i.uv对应的uv坐标
    //i.uv.x / num （一个单位内的x）
    //i.uv.y / num （一个单位内的y）

    //如果我现在都是空的加一个球，即再当前索引渲染一个对应贴图的球

    //

    //private Sprite[] EnergySp;

    private void Awake()
    {
        //EnergySp = Resources.LoadAll<Sprite>("heart");



        //energyHuds = GameObject.Find("Energy Hud").GetComponentsInChildren<Transform>();
        //val = 1f;
        //damage = 0.2f;
        //count = 1;
    }



    //private float val;
    //private float damage;
    //private Sprite[] energySp;
    //private Transform[] energyHuds;
    //private int count;



    //public void Init()
    //{
    //    InitHPHud();
    //    InitEnergyHud();
    //}

    //public void InitHPHud()
    //{
    //    //初始化血条和能量槽
    //    val = 1f;
    //    //hud.UpdateHud(val);    
    //}

    //public void InitEnergyHud()
    //{
    //    for (int i = 1; i < 4; i++)
    //    {
    //        energyHuds[i].GetComponentInChildren<SpriteRenderer>().sprite = energySp[1];  
    //    }
    //}

    //public void HPDecrese()
    //{
    //    val -= damage / 1f;
    //    //hud.UpdateHud(val);
    //}

    ////使用技能-3
    //public void EnergySlot()
    //{
    //    if (count != 4) return;
    //    InitEnergyHud();
    //}

    ////获得能量+1
    //public void GetEnergy()
    //{
    //    if (count >= 4) { return; }

    //    count += 1;
    //    energyHuds[count - 1].GetComponentInChildren<SpriteRenderer>().sprite = energySp[2];
    //}

    ////使用能量-1
    //public void UseEnergy()
    //{
    //    if (count <= 1) { return; }
    //    count -= 1;
    //    energyHuds[count].GetComponentInChildren<SpriteRenderer>().sprite = energySp[1];
    //}
}