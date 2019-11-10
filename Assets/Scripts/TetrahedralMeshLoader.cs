using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

enum LineType {
    NONE,
    VERTICES,
    TETRAHEDRA
}

// Used to load a .mesh file and its corresponding surface .obj file into a TetrahedralMesh object.
public class TetrahedralMeshLoader : MonoBehaviour {
    [SerializeField] private TetrahedralMesh tetMesh;

    private void Awake() {
        if (tetMesh == null)
            Debug.LogError("No TetrahedralMesh Component found.");
    }

    public void loadTetMesh(string filePath) {
        string surfaceFilePath = filePath + "__sf.obj";
        //check if .mesh and .obj files exist
        if (!File.Exists(filePath) || !File.Exists(surfaceFilePath)) {
            Debug.LogError("Mesh file or surface file does not exist. Make sure to have \n " + filePath + "and\n" + surfaceFilePath);
            return;
        }
        //init data holders
        List<Vector3> vertices = new List<Vector3>();
        List<int> tetrahedra = new List<int>();
        List<Vector3> surfaceVertices = new List<Vector3>();
        List<int> surfaceTriangles = new List<int>();
        //load data from files
        MeshImporter.import(filePath, out vertices, out tetrahedra);
        ObjImporter.import(surfaceFilePath, out surfaceVertices, out surfaceTriangles);
        DllInterface.getSingleton().setTetMeshData(vertices, tetrahedra, surfaceVertices, surfaceTriangles);
    //    DllInterface.getSingleton().startSimulation();
    }

    public void loadTetMesh() {
        string filePath = EditorUtility.OpenFilePanel("Load .mesh file", "", "mesh");
        loadTetMesh(filePath);
    }
}