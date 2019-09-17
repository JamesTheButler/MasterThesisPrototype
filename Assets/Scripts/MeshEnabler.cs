using UnityEngine;

public enum MeshType {
    ORIGINAL,
    ORIGINIAL_SURF, 
    DLL_SURF
}

public class MeshEnabler : MonoBehaviour {
    [SerializeField] private MeshRenderer originalMesh, surfaceMesh, dllSurfaceMesh;

    private void Start() {
        //show(MeshType.ORIGINAL);
    }

    public void show(MeshType meshType) {
        Debug.Log("MeshEnabler::show (" + meshType.ToString() + ")");
        hideAll();
        switch (meshType) {
            case MeshType.ORIGINAL:
                originalMesh.enabled = true;
                break;
            case MeshType.ORIGINIAL_SURF:
                surfaceMesh.enabled = true;
                break;
            case MeshType.DLL_SURF:
                dllSurfaceMesh.enabled = true;
                break;
        }
    }

    private void hideAll() {
        originalMesh.enabled = false;
        surfaceMesh.enabled = false;
        dllSurfaceMesh.enabled = false;
    }
}
