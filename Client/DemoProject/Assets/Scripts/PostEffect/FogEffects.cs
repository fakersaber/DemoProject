using UnityEngine;
using System.Collections;

public class FogEffects : PostEffectsBase
{
    public Shader FogShader;
    private Material Material;
    public Material material
    {
        get
        {
            Material = CheckShaderAndCreateMaterial(FogShader, Material);
            return Material;
        }
    }

    public Color FogColor = Color.white;

    //重写OnRenderImage方法
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (material != null)
        {
            material.SetColor("_Color", FogColor);
            Graphics.Blit(src, dest, material);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}