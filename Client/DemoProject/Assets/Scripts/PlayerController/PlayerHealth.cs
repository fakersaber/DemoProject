using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerHealth : MonoBehaviour
{
    private int weaponIndex;
    private int bodyIndex;
    private int headIndex;
    private int _PlayerId;
    private int Health = 20;
    public const float MaxHealth = 20f;
    public const int NormalDamage = 2;
    public const int CriticalDamage = 4;
    public const int ThunderDamage = 5;

    private NetWorkManager NetClass;
    private Material HealthBar;
    private PlayerEffectsManager EffectsManager;
    private CameraController cameraController;
    private LoadingManager loadingManager;
    private PlayerEnergyController SelfEnergyController;


    public int WeaponIndex
    {
        get { return weaponIndex; }
    }
    public int BodyIndex
    {
        get { return bodyIndex; }
    }

    public int HeadIndex
    {
        get { return headIndex; }
    }


    public int PlayerId
    {
        get { return _PlayerId; }
        set { _PlayerId = value; }
    }

    private void Awake()
    {
        var GameManager = GameObject.FindWithTag("GameManager");
        cameraController = GameObject.FindWithTag("MainCamera").GetComponent<CameraController>();
        loadingManager = GameManager.GetComponent<LoadingManager>();
        NetClass = GameManager.GetComponent<NetWorkManager>();
        EffectsManager = GameManager.GetComponent<PlayerEffectsManager>();
        weaponIndex = gameObject.GetComponents<PolygonCollider2D>()[0].GetHashCode();
        bodyIndex = gameObject.GetComponents<PolygonCollider2D>()[1].GetHashCode();
        headIndex = gameObject.GetComponents<PolygonCollider2D>()[2].GetHashCode();
        HealthBar = GetComponentsInChildren<SpriteRenderer>()[1].material;
        HealthBar.SetFloat("_CurHealth", Health / MaxHealth);
        SelfEnergyController = GetComponent<PlayerEnergyController>();
    }



    public void SubHp(int Damage)
    {
        if (Damage > 0 && Health > 0)
        {
            Health -= Damage;
            HealthBar.SetFloat("_CurHealth", Health / MaxHealth);
            if (Health <= 0)
            {
                //主端判断是否有多余的球
                if (NetClass.LocalPlayer == 1)
                {
                    int count = SelfEnergyController.EnergyList.Count;
                    for (int i = 0; i < count; ++i)
                    {
                        SelfEnergyController.ConsumeEnergySphere2();
                    }
                }

                EffectsManager.PlayerDead(_PlayerId, transform.position);                   
                if (!cameraController.isDead)
                    cameraController.isDead = _PlayerId == NetClass.LocalPlayer ? true : false;
                gameObject.SetActive(false);
                loadingManager.RankList.Add(_PlayerId);
                loadingManager.AliveSize--;
                if (loadingManager.AliveSize == 1)
                {
                    //最后两个人Add最后一个
                    for (int i = 1; i <= 4; ++i)
                    {
                        if (NetClass.AllPlayerInfo[i].activeSelf)
                        {
                            loadingManager.RankList.Add(i);
                            break;
                        }
                    }
                    loadingManager.SlowFrame = 50;
                }
            }
        }
    }
}
