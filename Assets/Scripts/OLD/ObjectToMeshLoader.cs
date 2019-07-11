using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[System.Obsolete("Not used. Surface mesh generation too hard.")]
class ObjectToMeshLoader : MonoBehaviour {
    [SerializeField] private GameObject meshGO;
    [SerializeField] private Material meshMat;

    private void Start() {
        Mesh mesh = new Mesh();
        MeshFilter filter = meshGO.AddComponent<MeshFilter>();
        filter.mesh = mesh;
        MeshRenderer renderer = meshGO.AddComponent<MeshRenderer>();
        renderer.material = meshMat;

        List<Vector3> vertices = new List<Vector3>();
        List<int> tris = new List<int>();

        string filePath = EditorUtility.OpenFilePanel("Load .obj file", "", "obj");
        ObjImporter.import(filePath, out vertices, out tris);

        mesh.vertices = vertices.ToArray();
        mesh.triangles = tris.ToArray();

        /*NormalDrawer normalDrawer = meshGO.AddComponent<NormalDrawer>();
        normalDrawer.go = meshGO;
        normalDrawer.material = meshMat;*/
    }
}
