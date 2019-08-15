using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HealthBar
{
    public const float width = 2f;
    public const float height = 0.1f;

    public static Vector3[] Edges =
    {
        new Vector3(-width,-height,0f),
        new Vector3(-width,height,0f),
        new Vector3(width,height,0f),
        new Vector3(width,-height,0f)
    };

    public static Color[] VertextColor =
    {
        Color.green,
        Color.green,
        Color.green,
        Color.green
    };


    //左下顺时针
    public static int[] VertextIndex =
    {
        0,1,2, 
        0,2,3
    };
}
