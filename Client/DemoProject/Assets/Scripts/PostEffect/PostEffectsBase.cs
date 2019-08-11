using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent (typeof(Camera))]
public class PostEffectsBase : MonoBehaviour
{
    private Material curMaterial;
    public Shader curShader;


    void Start()
    {
        CheckResources();
    }


    public Material material
    {
        get
        {
            if (curMaterial == null)
            {
                curMaterial = new Material(curShader);
                curMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return curMaterial;
        }
    }


    protected bool CheckResources()
    {
        if(SystemInfo.supportsImageEffects == false || SystemInfo.supportsRenderTextures == false)
        {
            Debug.LogWarning("This platform does not support");
            return false;
        }
        return true;
    }



    private void OnDisable()
    {
        if (curMaterial != null)
        {
            DestroyImmediate(curMaterial);
        }
    }
}
