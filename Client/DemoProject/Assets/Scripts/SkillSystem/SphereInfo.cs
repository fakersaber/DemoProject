using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereInfo : MonoBehaviour
{
    private int _SphereId;
    private int _Type;

    public int SphereId
    {
        get { return _SphereId; }
        set { _SphereId = value; }
    }


    public int Type
    {
        get { return _Type; }
        set { _Type = value; }
    }

}