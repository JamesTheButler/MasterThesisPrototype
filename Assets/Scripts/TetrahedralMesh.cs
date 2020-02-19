using System;
using UnityEngine;

public class TetrahedralMesh : MonoBehaviour, ICollisionEventHandler {
    [SerializeField] private GameObject carGO;
    [SerializeField] private GameObject carModelGO;
    [SerializeField] private GameObject tetMeshGameObject;
    [SerializeField] private GameObject surfaceMeshGO;

    // Returns the translation and rotation of the tet mesh using the parameters as output.
    public void getTransforms(out Vector3 translation, out Vector3 rotation) {
        translation = carGO.transform.position;
        rotation = carGO.transform.rotation.eulerAngles;
    }

    // Returns surface vertices.
    public Vector3[] getSurfaceVertices() {
        return carModelGO.GetComponent<MeshFilter>().mesh.vertices;
    }

    // Update the model of the car.
    public void updateCarModel(Vector3[] newVertices) {
        carModelGO.GetComponent<MeshFilter>().mesh.vertices = newVertices;
    }

    // Update the tet mesh surface model.
    public void updateSurfaceModel(Vector3[] newVertices) {
        surfaceMeshGO.GetComponent<MeshFilter>().mesh.vertices = newVertices;
    }

    public void setTetMeshSurface(Vector3[] surfaceVertices, int[] surfaceTriangles) {
        Mesh mesh = new Mesh();
        mesh.vertices = surfaceVertices;
        mesh.triangles = surfaceTriangles;
        surfaceMeshGO.GetComponent<MeshFilter>().mesh = mesh;
    }

    public void onTriggerStay(Collider otherCollider) {
        if (otherCollider.tag == "Obstacle") {
            DllInterface.getSingleton().getCollisionResult(otherCollider.gameObject.GetComponent<MyCollider>().getId());
        }
    }
}