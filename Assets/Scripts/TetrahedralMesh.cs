using System.Collections.Generic;
using UnityEngine;

public class TetrahedralMesh : MonoBehaviour, ICollisionEventHandler {
    [SerializeField] private GameObject carGO;
    [SerializeField] private GameObject carModelGO;
    [SerializeField] private GameObject tetMeshGameObject;
    [SerializeField] private Material tetMeshMaterial;
    [SerializeField] private GameObject surfaceMeshGO;

    private Mesh mesh;
    private MeshFilter mf;
    private MeshRenderer mr;

    private int[] surfTris;

    private void Start() {
        mesh = new Mesh();
        mf= surfaceMeshGO.AddComponent<MeshFilter>();
        mr = surfaceMeshGO.GetComponent<MeshRenderer>();
    }

    // Returns the translation and rotation of the tet mesh using the parameters as output.
    public void getTransforms(out Vector3 translation, out Vector3 rotation) {
        translation = carGO.transform.position;
        rotation = carGO.transform.rotation.eulerAngles;
    }

    // for manual import
    // Sets up the surface mesh.
    public void setupSurface(Vector3[] surfaceVertices, int[] surfaceTriangles) {
        mr.material = tetMeshMaterial;
        mesh.vertices = surfaceVertices;
        mesh.triangles = surfaceTriangles;
    }

    // for manual import
    // Update surface mesh.
    public void setSurfaceVertices(Vector3[] newVertices) {
        mesh.vertices = newVertices;
    }

    // for automatic import
    // Returns surface vertices.
    public Vector3[] getSurfaceVertices() {
        Debug.Log(carModelGO.GetComponent<MeshFilter>().mesh.vertices[10]);
        Debug.Log(carModelGO.GetComponent<MeshFilter>().mesh.vertices[12]);
        Debug.Log(carModelGO.GetComponent<MeshFilter>().mesh.vertices[13]);
        return carModelGO.GetComponent<MeshFilter>().mesh.vertices;
    }

    // for automatic import
    // Returns surface vertices.
    public Vector3[] getScaledSurfaceVertices(float scale) {
        List<Vector3> scaledVerts = new List<Vector3>();
        Vector3[] meshVerts = carModelGO.GetComponent<MeshFilter>().mesh.vertices;
        for (int i=0; i< meshVerts.Length; i++) {
            scaledVerts.Add(meshVerts[i] * scale);
        }
        return scaledVerts.ToArray();
    }

    // for automatic import
    // Update the model of the car.
    public void updateCarModel(Vector3[] newVertices) {
        Mesh carMesh = carModelGO.GetComponent<MeshFilter>().mesh;
        Mesh mesh = new Mesh();
        Debug.Log("carmesh verts "+carMesh.vertices.Length);
        Debug.Log("new verts "+ newVertices.Length);
        mesh.vertices = newVertices;
        mesh.triangles = carMesh.triangles;
        carMesh = mesh;
    }

    public void onTriggerStay(Collider otherCollider) {
        if (otherCollider.tag == "Obstacle") {
            DllInterface.getSingleton().getCollisionResult(otherCollider.gameObject.GetComponent<MyCollider>().getId());
        }
    }
}