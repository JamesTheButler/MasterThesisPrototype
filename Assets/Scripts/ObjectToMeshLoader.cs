using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
    //    List<Vector3> normals = new List<Vector3>();

        string filePath = EditorUtility.OpenFilePanel("Load .obj file", "", "obj");
    //    ObjImporter.import(filePath, out vertices, out tris, out normals);
        ObjImporter.import(filePath, out vertices, out tris);

        mesh.vertices = vertices.ToArray();
        mesh.triangles = tris.ToArray();
        //    mesh.normals = normals.ToArray();

    //    Mesh newMesh = duplicateMeshWithIndividualVertices(mesh);
        /*NormalDrawer normalDrawer = meshGO.AddComponent<NormalDrawer>();
        normalDrawer.go = meshGO;
        normalDrawer.material = meshMat;*/
    }

    private Mesh duplicateMeshWithIndividualVertices(Mesh oldMesh) {
        Mesh newMesh = new Mesh();
        List<Vector3> oldVerts = new List<Vector3>(oldMesh.vertices);
        List<int> oldTris = new List<int>(oldMesh.triangles);
        List<Vector3> newVerts = new List<Vector3>();
        List<int> newTris = new List<int>();
        List<Vector3> newNormals = new List<Vector3>();
        //copy vertices and tris


        //generate normals
        calculateNormals(newVerts, newTris, out newNormals);
        //write and return new mesh info
        newMesh.vertices = newVerts.ToArray();
        newMesh.triangles = newTris.ToArray();
        newMesh.normals = newNormals.ToArray();
        return newMesh;
    }

    private static void calculateNormals(List<Vector3> verts, List<int> tris, out List<Vector3> normals) {
        normals = new List<Vector3>();
        //initialize empty normals
        for (int i = 0; i < verts.Count; i++) {
            normals.Add(new Vector3(0, 0, 0));
        }
        Debug.Log(tris.Count + " /3 = " + tris.Count / 3);
        //for (int tri = 0; tri < tris.Count / 3; tri++) {
        for (int tri = 0; tri < 3; tri++) {
            Vector3 vec1, vec2;
            vec1 = verts[tris[tri * 3 + 2]] - verts[tris[tri * 3]];
            vec2 = verts[tris[tri * 3 + 1]] - verts[tris[tri * 3]];

            Vector3 faceNormal = Vector3.Normalize(Vector3.Cross(vec1, vec2));

            for (int triVertex = 0; triVertex <= 2; triVertex++) {
                int vertexId = tris[tri * 3 + triVertex];
                // Debug.Log(normals.Count + ", " + vertexId);
                normals[vertexId] = faceNormal;
            }
        }
    }
}
