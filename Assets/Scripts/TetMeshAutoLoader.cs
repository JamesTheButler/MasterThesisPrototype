using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetMeshAutoLoader : MonoBehaviour {
    [SerializeField] string filePath;

	void Start () {
        GetComponent<TetrahedralMeshLoader>().loadTetMesh(filePath);
	}
}
