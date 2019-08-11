using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class FogEffects : PostEffectsBase
{
    [Range(0, 100)]
    public float DFogDensity = 1f; //传统雾浓度
    [Range(0, 20)]
    public float VFogDensity = 1f; //高低雾系数
    [Range(0, 100)]
    public float FogMass = 24f; //雾的沉积度
    public float StartY = 0f;
    public float DFogHieght = 0f;
    public float DFogMass = 24f;
    public Color LightColor = Color.white;
    public Color DFogColorFar = Color.white;
    public Color DFogColorNear = Color.white;
    public Color VFogColorLow = Color.white;
    public Color VFogColorHigh = Color.white;



    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material != null)
        {
            material.SetColor("_LightColor", LightColor);
            material.SetFloat("_DFogDensity", DFogDensity);
            material.SetFloat("_VFogDensity", VFogDensity);
            material.SetColor("_DFogColorFar", DFogColorFar);
            material.SetColor("_DFogColorNear", DFogColorNear);
            material.SetColor("_VFogColorLow", VFogColorLow);
            material.SetColor("_VFogColorHigh", VFogColorHigh);
            material.SetFloat("_FogMass", FogMass);
            material.SetFloat("_StartY", StartY);
            material.SetFloat("_DFogHeight", DFogHieght);
            material.SetFloat("_DFogMass", DFogMass);
            Graphics.Blit(source, destination, material);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }


}
