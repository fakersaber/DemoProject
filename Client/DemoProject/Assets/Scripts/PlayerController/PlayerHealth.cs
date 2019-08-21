using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerHealth : MonoBehaviour
{
    private int weaponIndex;
    private int bodyIndex;
    private float Health = 10;
    public const float MaxHealth = 10f;
    public const int NormalDamage = 2;
    public const int ThunderDamage = 5;


    private NetWorkManager NetClass;
    private Material HealthBar;



    public int WeaponIndex
    {
        get { return weaponIndex; }
    }
    public int BodyIndex
    {
        get{ return bodyIndex; }
    }


    private void Awake()
    {
        NetClass = GameObject.FindWithTag("GameManager").GetComponent<NetWorkManager>();
        weaponIndex = gameObject.GetComponents<PolygonCollider2D>()[0].GetHashCode();
        bodyIndex = gameObject.GetComponents<PolygonCollider2D>()[1].GetHashCode();
        HealthBar = GetComponentsInChildren<SpriteRenderer>()[1].material;
        HealthBar.SetFloat("_CurHealth", Health / MaxHealth);
    }



    public void SubHp(int Damage)
    {
        if (Damage > 0)
        {
            Health -= Damage;
            HealthBar.SetFloat("_CurHealth", Health/MaxHealth);
            if (Health <= 0f)
            {
                gameObject.SetActive(false);
            }
        }
    }

}
