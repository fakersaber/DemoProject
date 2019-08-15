using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class HealthBarMesh : MonoBehaviour
{
    private Vector3[] vertices = new Vector3[4];
    private int[] triangles = new int[6];
    private Color[] colors = new Color[4];


    private MeshFilter CurFilterMesh;
    private MeshRenderer CurMeshRender;

    private void Awake()
    {
        CurFilterMesh = gameObject.GetComponent<MeshFilter>();
        CurMeshRender = gameObject.GetComponent<MeshRenderer>();


        CurFilterMesh.mesh.vertices = HealthBar.Edges;
        CurFilterMesh.mesh.colors = HealthBar.VertextColor;
        CurFilterMesh.mesh.triangles = HealthBar.VertextIndex;

        CurMeshRender.shadowCastingMode = ShadowCastingMode.Off;
        CurMeshRender.receiveShadows = false;
        CurMeshRender.lightProbeUsage = LightProbeUsage.Off;
        CurMeshRender.reflectionProbeUsage = ReflectionProbeUsage.Off;
    }


    public void UpdateHealthBar()
    {

    }
}