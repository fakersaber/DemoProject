using UnityEngine;
using System.Collections;

//在编辑器状态下执行该脚本
[ExecuteInEditMode]
//刚需组件（Camera）
[RequireComponent(typeof(Camera))]
public class PostEffectsBase : MonoBehaviour
{

    // 在Start()中调用
    protected void CheckResources()
    {
        bool isSupported = CheckSupport();

        if (isSupported == false)
        {
            NotSupported();
        }
    }

    // 平台渲染纹理与屏幕特效支持检测
    protected bool CheckSupport()
    {
        if (SystemInfo.supportsImageEffects == false || SystemInfo.supportsRenderTextures == false)
        {
            Debug.LogWarning("This platform does not support image effects or render textures.");
            return false;
        }

        return true;
    }

    // 当不支持的时候，将脚本的enabled设置为false
    protected void NotSupported()
    {
        enabled = false;
    }

    protected void Start()
    {
        CheckResources();
    }

    // 检测Material和Shader，在派生类中调用，绑定材质和shader
    protected Material CheckShaderAndCreateMaterial(Shader shader, Material material)
    {
        if (shader == null)
        {
            return null;
        }

        if (shader.isSupported && material && material.shader == shader)
            return material;

        if (!shader.isSupported)
        {
            return null;
        }
        else
        {
            material = new Material(shader);
            material.hideFlags = HideFlags.DontSave;
            if (material)
                return material;
            else
                return null;
        }
    }
}
