using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

// Used to load a .mesh file and its corresponding surface .obj file into a TetrahedralMesh object.
public class TetrahedralMeshLoader : MonoBehaviour {
    [SerializeField] private TetrahedralMesh tetMesh;

    private void Awake() {
        if (tetMesh == null)
            Debug.LogError("No TetrahedralMesh Component found.");
    }

    public void loadTetMesh(string filePath, bool doAutomaticSurfaceLoad) {
        //check if .mesh and .obj files exist
        if (!File.Exists(filePath) ) {
            Debug.LogError("Mesh file does not exist. Make sure to have \n " + filePath );
            return;
        }
        //init data holders
            // tet mesh
        List<Vector3> tetMeshVertices = new List<Vector3>();
        List<int> tetMeshTetrahedra = new List<int>();
            //surface
        List<Vector3> surfaceVertices = new List<Vector3>();
        List<int> surfaceTriangles = new List<int>();
        //load tet mesh data from file; pass to dll
        MeshImporter.import(filePath, out tetMeshVertices, out tetMeshTetrahedra);
        DllInterface.getSingleton().setTetMeshData(tetMeshVertices, tetMeshTetrahedra);

        if (doAutomaticSurfaceLoad) {       
            // passes surface vertices of the car model to the dll.
            //DllInterface.getSingleton().setSurfaceData(tetMesh.getScaledSurfaceVertices(100.0f));
            DllInterface.getSingleton().setSurfaceData(tetMesh.getSurfaceVertices());
        } else {
            // load surface from file; pass to dll; set up surface mesh
            ObjImporter.import(filePath + "__sf.obj", out surfaceVertices, out surfaceTriangles);
            DllInterface.getSingleton().setSurfaceData(surfaceVertices.ToArray());
            tetMesh.setupSurface(surfaceVertices.ToArray(), surfaceTriangles.ToArray());
        }
        DllInterface.getSingleton().startSimulation();
    }

    public void loadTetMesh() {
        string filePath = EditorUtility.OpenFilePanel("Load .mesh file", "", "mesh");
        loadTetMesh(filePath, false);
    }
}