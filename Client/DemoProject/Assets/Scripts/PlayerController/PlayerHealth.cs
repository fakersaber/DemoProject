using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerHealth : MonoBehaviour
{
    private int weaponIndex;
    private int bodyIndex;
    public float Health = 10;
    public const float MaxHealth = 10f;
    //public int Damage = 2;
    private NetWorkManager NetClass;
    public GameObject WeaponEffect;
    public GameObject AttackedEffect;
    public GameObject PlayerCollidEffct;
    private ParticleSystem WeaponToWeapon;
    private ParticleSystem BodyToBody;
    private ParticleSystem BodyToWeapon;
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
        WeaponToWeapon = WeaponEffect.GetComponent<ParticleSystem>();
        BodyToBody = PlayerCollidEffct.GetComponent<ParticleSystem>();
        BodyToWeapon = AttackedEffect.GetComponent<ParticleSystem>();

        HealthBar = GetComponentsInChildren<SpriteRenderer>()[1].material;
        HealthBar.SetFloat("_CurHealth", Health / MaxHealth);
    }



    public void SubHp(int Damage)
    {
        if (Damage > 0)
        {
            Health -= Damage;
            HealthBar.SetFloat("_CurHealth", Health/MaxHealth);
            if (Health <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }


    public void PlayerSpecialEffects(int EffectsIndex,Vector2 PlayPosition)
    {
        switch(EffectsIndex)
        {
            case (int)SpecialEffects.BADYTOBADY:
                BodyToBody.transform.position = PlayPosition;
                BodyToBody.Play();
                break;
            case (int)SpecialEffects.WEAPONTOWEAPON:
                WeaponToWeapon.transform.position = PlayPosition;
                WeaponToWeapon.Play();
                break;
            case (int)SpecialEffects.BADYTOWEAPON:
                BodyToWeapon.transform.position = PlayPosition;
                BodyToWeapon.Play();
                break;
        }
    }
}
