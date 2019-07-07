using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshThings : MonoBehaviour {
    [SerializeField] private GameObject spherePrefab;
    [SerializeField] private GameObject sphereParent;

    [SerializeField] private List<MeshFilter> meshes;
    
    private void generateSpheres() {
        //TODO: WAY too many vertecies in the sky car mesho
        foreach(MeshFilter mesh in meshes) {
            if (mesh == null)
                continue;
            foreach (Vector3 vertex in mesh.mesh.vertices)
                Instantiate(spherePrefab, vertex, new Quaternion(), sphereParent.transform);
        }
        //Debug.Log(meshes[0].mesh.vertexCount);
    }

	// Use this for initialization
	void Start () {
        generateSpheres();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
