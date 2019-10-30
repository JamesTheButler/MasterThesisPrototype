using UnityEngine;

public class TetMeshAutoLoader : MonoBehaviour {
    [SerializeField] string filePath;
    //TODO: find better solution and do it in void Start()
    int i = 0;

    void Update () {
        if (i == 0) {
            GetComponent<TetrahedralMeshLoader>().loadTetMesh(filePath);
            i = 1;
        }
	}
}