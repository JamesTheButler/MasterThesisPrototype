using UnityEngine;

public class TetrahedralMesh : MonoBehaviour, ICollisionEventHandler {
    [SerializeField] private GameObject carGO;
    [SerializeField] private GameObject carModelGO;
    [SerializeField] private GameObject tetMeshGameObject;
    [SerializeField] private Material tetMeshMaterial;
    [SerializeField] private GameObject surfaceMeshGO;

    public bool showImportedMesh;

    private Mesh mesh;
    private MeshFilter mf;
    private MeshRenderer mr;

    private int[] surfTris;

    private void Start() {
        mesh = new Mesh();
        mf= surfaceMeshGO.AddComponent<MeshFilter>();
        mr = surfaceMeshGO.GetComponent<MeshRenderer>();
    }

    /// <summary>
    /// Returns the translation and rotation of the tet mesh using the parameters as output.
    /// </summary>
    public void getTransforms(out Vector3 translation, out Vector3 rotation) {
        translation = carGO.transform.position;
        rotation = carGO.transform.rotation.eulerAngles;
    }
    
    //TODO: update original model rather than imported model
    public void setupSurface(Vector3[] surfaceVertices, int[] surfaceTriangles) {
        if (showImportedMesh) {
            mr.material = tetMeshMaterial;
            surfTris = surfaceTriangles;
            updateSurface(surfaceVertices);
        }
        //updateCarModel(surfaceVertices);
    }

    public void updateSurface(Vector3[] newVertices) {
        if (showImportedMesh) {
            mesh = new Mesh();
            mf.mesh = mesh;
            mesh.vertices = newVertices;
            mesh.triangles = surfTris;
        }
    }

    public void onTriggerStay(Collider otherCollider) {
        if (otherCollider.tag == "Obstacle") {
            //DllInterface.getSingleton().getCollisionResult(otherCollider.gameObject.GetComponent<MyCollider>().getId());
        }
    }

    public void updateCarModel(Vector3[] newVertices) {
        Mesh carMesh = carModelGO.GetComponent<MeshFilter>().mesh;
        Mesh mesh = new Mesh();
        Debug.Log("carmesh verts "+carMesh.vertices.Length);
        Debug.Log("new verts "+ newVertices.Length);
        mesh.vertices = newVertices;
        mesh.triangles = carMesh.triangles;
        carMesh = mesh;
    }
}