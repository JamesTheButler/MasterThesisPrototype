using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshInfo : MonoBehaviour {
    public MeshFilter meshFilter;
    public bool showDetails;

	void Start () {
        Debug.Log("vertices "+meshFilter.mesh.vertices.Length);
        if (showDetails) {
            string result = "";
            for (int i = 0; i < meshFilter.mesh.vertices.Length; i++)
                result += meshFilter.mesh.vertices[i].ToString();
            Debug.Log("vertices: [" +result+ "]");
        }
        Debug.Log("tris "+meshFilter.mesh.triangles.Length/3);
        Debug.Log("uv "+meshFilter.mesh.uv.Length);
        Debug.Log("normals" + meshFilter.mesh.normals.Length);
        if (showDetails) {
            string result = "";
            for (int i = 0; i < meshFilter.mesh.normals.Length; i++)
                result += meshFilter.mesh.normals[i].ToString();
            Debug.Log("normals: [" + result + "]");
        }
	}

}
