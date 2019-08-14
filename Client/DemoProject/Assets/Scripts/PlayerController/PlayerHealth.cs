using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerHealth : MonoBehaviour
{
    private int weaponIndex;
    private int bodyIndex;
    public int Health = 100;
    //public int Damage = 2;
    private NetWorkManager NetClass;
    public GameObject WeaponEffect;
    public GameObject AttackedEffect;
    public GameObject PlayerCollidEffct;
    private ParticleSystem WeaponToWeapon;
    private ParticleSystem BodyToBody;
    private ParticleSystem BodyToWeapon;



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
    }



    public void SubHp(int Damage)
    {
        if (Damage > 0)
        {
            Health -= Damage;
            //发送扣血消息
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
