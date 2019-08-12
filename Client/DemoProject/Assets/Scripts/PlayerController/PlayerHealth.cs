using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerHealth : MonoBehaviour
{
    private int weaponIndex;
    private int bodyIndex;
    public int Health = 10;
    //public int Damage = 2;
    private NetWorkManager NetClass;



    public int WeaponIndex
    {
        get { return weaponIndex; }
    }
    public int BodyIndex
    {
        get
        {
            return bodyIndex;
        }
    }

    private void Awake()
    {
        NetClass = GameObject.Find("GameManager").GetComponent<NetWorkManager>();
        weaponIndex = gameObject.GetComponents<PolygonCollider2D>()[0].GetHashCode();
        bodyIndex = gameObject.GetComponents<PolygonCollider2D>()[1].GetHashCode();
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


    public void PlayerSpecialEffects(int EffectsIndex)
    {
        switch(EffectsIndex)
        {
            case (int)SpecialEffects.BADYTOBADY:

                break;
            case (int)SpecialEffects.WEAPONTOWEAPON:

                break;
            case (int)SpecialEffects.BADYTOWEAPON:

                break;
            //case (int)SpecialEffects.WEAPONTOBODY:
            //    break;
        }
    }
}
