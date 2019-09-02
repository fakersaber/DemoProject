
using UnityEngine;




static class MathTool
{
    //将right的轴映射为up方向，返回范围为(-180，180)的角度
    public static float MappingRotation(Vector2 dir)
    {
        //y = sina = sin(π/2 - b) = cosb
        float sita = Mathf.Acos(dir.normalized.y) * 180f / Mathf.PI;
        if (Vector2.Dot(dir, Vector2.right) > 0f)
            sita = -sita;
        return sita;
    }
}

