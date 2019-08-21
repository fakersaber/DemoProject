using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerEffectsManager : MonoBehaviour
{
    public GameObject WeaponEffect;
    public GameObject AttackedEffect;
    public GameObject PlayerCollidEffct;
    private class PlayerEffects
    {
        public ParticleSystem WeaponToWeapon;
        public ParticleSystem BodyToBody;
        public ParticleSystem BodyToWeapon;
    }
    private Dictionary<int, PlayerEffects> AllEffectsInfo = new Dictionary<int, PlayerEffects>(4);


    public void InitPlayerEffects(int PlayerId)
    {
        PlayerEffects CurEffects = new PlayerEffects();
        CurEffects.WeaponToWeapon = Instantiate(WeaponEffect).GetComponent<ParticleSystem>();
        CurEffects.BodyToWeapon = Instantiate(AttackedEffect).GetComponent<ParticleSystem>();
        CurEffects.BodyToBody = Instantiate(PlayerCollidEffct).GetComponent<ParticleSystem>();
        AllEffectsInfo.Add(PlayerId, CurEffects);
    }


    public void PlayerSpecialEffects(int PlayerId,int EffectsIndex, Vector2 PlayPosition)
    {
        switch (EffectsIndex)
        {
            case (int)SpecialEffects.BADYTOBADY:
                var BodyToBody = AllEffectsInfo[PlayerId].BodyToBody;
                BodyToBody.transform.position = PlayPosition;
                BodyToBody.Play();
                break;
            case (int)SpecialEffects.WEAPONTOWEAPON:
                var WeaponToWeapon = AllEffectsInfo[PlayerId].WeaponToWeapon;
                WeaponToWeapon.transform.position = PlayPosition;
                WeaponToWeapon.Play();
                break;
            case (int)SpecialEffects.BADYTOWEAPON:
                var BodyToWeapon = AllEffectsInfo[PlayerId].BodyToWeapon;
                BodyToWeapon.transform.position = PlayPosition;
                BodyToWeapon.Play();
                break;
        }
    }

}
