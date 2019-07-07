using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshInfo : MonoBehaviour {
    public MeshFilter mf;
	
	void Start () {
        Debug.Log("vertices "+mf.mesh.vertices.Length);
        Debug.Log("tris "+mf.mesh.triangles.Length);
        Debug.Log("uv "+mf.mesh.uv.Length);
        Debug.Log("normals" + mf.mesh.normals.Length);
        Debug.Log(mf.mesh.normals[0]);
	}
}
