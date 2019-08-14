using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnergySpherePool : MonoBehaviour
{
    //总数守恒
    public int InitSize = 5;
    private Dictionary<int, GameObject> AllSpherePoll = new Dictionary<int, GameObject>(15);

    //cache component
    private Dictionary<int, SphereInfo> AllSphereInfo = new Dictionary<int, SphereInfo>(15);

    public GameObject RedEnergySphere;
    public GameObject BlueEnergySphere;
    public GameObject YellowEnergySphere;


    public void InitPool(Vector2 Position,int Type,int SphereId)
    {
        GameObject TempObj = null;
        switch (Type)
        {
            case (int)SphereType.SPHERE_RED:
                TempObj = Instantiate(RedEnergySphere);
                break;
            case (int)SphereType.SPHERE_BLUE:
                TempObj = Instantiate(BlueEnergySphere);
                break;
            case (int)SphereType.SPHERE_YELLOW:
                TempObj = Instantiate(YellowEnergySphere);
                break;
        }
        TempObj.transform.position = Position;
        AllSpherePoll.Add(SphereId, TempObj);
        var CurComponent = TempObj.GetComponent<SphereInfo>();
        CurComponent.SphereId = SphereId;
        CurComponent.Type = Type;
        AllSphereInfo.Add(SphereId, CurComponent);
    }

    public SphereInfo GetSphereInfo(int SphereIndex)
    {
        return AllSphereInfo[SphereIndex];
    }

    public void GeneratorNewSphere(int SphereIndex, Vector2 Position)
    {
        AllSpherePoll[SphereIndex].transform.position = Position;
        AllSpherePoll[SphereIndex].SetActive(true);
    }

    public void Collect(int SphereIndex)
    {
        AllSpherePoll[SphereIndex].SetActive(false);
    }

}
