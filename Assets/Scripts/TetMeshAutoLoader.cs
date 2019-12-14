using UnityEngine;

public class TetMeshAutoLoader : MonoBehaviour {
    [SerializeField] string filePath;
    [SerializeField] string fileName;

    //TODO: find better solution and do it in void Start()
    bool isFirstFrame = true;

    void Update () {
        if (isFirstFrame) {
            if (!filePath.EndsWith("\\"))
                filePath+="\\";
            GetComponent<TetrahedralMeshLoader>().loadTetMesh(filePath+fileName+ ".obj.mesh", true);
            isFirstFrame = false;
        }
	}
}