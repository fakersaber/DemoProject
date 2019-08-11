using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class DistorEffects : PostEffectsBase
{
    public Texture NoiseTexture;
    [Range(0.0f, 1.0f)]
    public float DistortTimeFactor = 0.1f; //采样噪音图的幅度 
    [Range(0.0f, 1.0f)]
    public float LuminosityAmount = 0.15f; // 扭曲系数



    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if(material != null)
        {
            material.SetFloat("_LuminosityAmount", LuminosityAmount);
            material.SetFloat("_DistortTimeFactor", DistortTimeFactor);
            material.SetTexture("_NoiseTex", NoiseTexture);
            Graphics.Blit(source, destination, material);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }



}
