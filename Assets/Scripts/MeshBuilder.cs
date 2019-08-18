using System;
using UnityEngine;

public class MeshBuilder : MonoBehaviour {
    [SerializeField] private GameObject target;
    [SerializeField] private Material material;
    private MeshFilter surfaceMeshFilter;

    public void init(Vector3[] vertices, int[] triangles) {
        Mesh mesh = new Mesh();
        //MeshFilter filter = target.AddComponent<MeshFilter>();
        //filter.mesh = mesh;
        surfaceMeshFilter = target.AddComponent<MeshFilter>();
        surfaceMeshFilter.mesh = mesh;
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        //Debug.Log("MeshBuilder -- init");
    }

    public void setVertexData(Vector3[] vertices) {
        surfaceMeshFilter.mesh.vertices = vertices;
    }

    public void setTransforms(Vector3 translation, Quaternion rotation){
       // target.transform.position = translation;
        //target.transform.rotation = rotation;
    }
}