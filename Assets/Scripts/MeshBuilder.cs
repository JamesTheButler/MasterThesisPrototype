using UnityEngine;

public class MeshBuilder : MonoBehaviour {
    [SerializeField] private GameObject target;
    [SerializeField] private Material material;
    private MeshFilter surfaceMeshFilter;

    public void init(Vector3[] vertices, int[] triangles) {
        Mesh mesh = new Mesh();
        // MeshFilter filter = target.AddComponent<MeshFilter>();
        // filter.mesh = mesh;
        surfaceMeshFilter = target.AddComponent<MeshFilter>();
        surfaceMeshFilter.mesh = mesh;
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        Debug.Log("MeshBuilder.init: 1st vert: " + vertices[0]);
        Debug.Log("MeshBuilder.init: 10th vert: " + vertices[10]);
        Debug.Log("MeshBuilder.init: 100th vert: " + vertices[100]);
    }

    public void setVertexData(Vector3[] vertices) {
        Debug.Log("setVertexData: 1st vert: "+vertices[0]);
        Debug.Log("setVertexData: 10th vert: "+vertices[10]);
        Debug.Log("setVertexData: 1700th vert: "+vertices[1700]);
        surfaceMeshFilter.mesh.vertices = vertices;
    }
}