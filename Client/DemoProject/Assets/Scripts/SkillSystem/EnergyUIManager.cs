using UnityEngine;
using System.Collections;

public class EnergyUIManager : MonoBehaviour
{
    private Sprite[] EnergyResource;
    private SpriteRenderer[] EnergyRender;
    private int CurIndex = 0;

    private void Awake()
    {
        EnergyResource = Resources.LoadAll<Sprite>("EnergyUI");
        EnergyRender = GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < EnergyRender.Length; ++i)
        {
            EnergyRender[i].sprite = EnergyResource[3];
        }
    }


    public void CollectSphere(int type)
    {
        switch (type)
        {
            case (int)SphereType.SPHERE_RED:
                EnergyRender[CurIndex].sprite = EnergyResource[1];
                break;
            case (int)SphereType.SPHERE_BLUE:
                EnergyRender[CurIndex].sprite = EnergyResource[0];
                break;
            case (int)SphereType.SPHERE_PURPLE:
                EnergyRender[CurIndex].sprite = EnergyResource[2];
                break;
        }
        CurIndex++;
    }


    public void ConsumeSphere()
    {
        EnergyRender[CurIndex-1].sprite = EnergyResource[3];
        CurIndex--;
    }

}
