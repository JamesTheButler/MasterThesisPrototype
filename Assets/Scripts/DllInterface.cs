using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class DllInterface : MonoBehaviour {
    public GameObject infoGO;

    [SerializeField] private Slider iterationSlider;
    [SerializeField] private Slider plasticitySlider;
    [SerializeField] private TetrahedralMesh tetMesh;
    [SerializeField] private Text solverDeltaTimeText;
    [SerializeField] private Text collisionCountText;
    [SerializeField] private Transform carTransform;

    [SerializeField] private string filePath;
    [SerializeField] private string backupFilePath;
    [SerializeField] private string fileName;

    [SerializeField] private bool useSerialInit = false;
    [SerializeField] private bool useSurfaceToTetMap = false;
    [SerializeField] private bool useInitFromFile = false;
    [SerializeField] private bool useStoreData = false;

    private static DllInterface singleton;
    public static DllInterface getSingleton() { return singleton; }

    private int vertCount, tetCount, surfVertCount;
    private bool isSimulating=false;

    private List<int> collisions;

    // DLL Methods
    #region DLL definition
    ///getters
    // tetrahedra
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getTetrahedra(IntPtr tets);
    [DllImport("PlasticDeformationDll")]
    private static extern int dll_getTetrahedronCount();
    // vertices
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getVertices(IntPtr verts);
    [DllImport("PlasticDeformationDll")]
    private static extern int dll_getVertexCount();
    //colliders
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getColliders(IntPtr positions, IntPtr sizes, IntPtr types);
    [DllImport("PlasticDeformationDll")]
    private static extern int dll_getCollisionCount();
    //surface data
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getSurfaceVertices(IntPtr verts);
    [DllImport("PlasticDeformationDll")]
    private static extern int dll_getSurfaceVertexCount();
    // barycentric data
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getBarycentricCoordinates(IntPtr barycentricTetIds);
    [DllImport("PlasticDeformationDll")]
    private static extern int dll_getBarycentricCoordCount();
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getBarycentricTetIds(IntPtr barycentricTetIds);
    [DllImport("PlasticDeformationDll")]
    private static extern int dll_getBarycentricTetIdCount();
    //constraint info
    [DllImport("PlasticDeformationDll")]
    private static extern int dll_getDistanceConstraintCount();
    [DllImport("PlasticDeformationDll")]
    private static extern int dll_getVolumeConstraintCount();
    [DllImport("PlasticDeformationDll")]
    private static extern int dll_getConstraintCount();
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getConstraintRestValues(IntPtr output);
    [DllImport("PlasticDeformationDll")]
    private static extern int dll_getDeltasCount();
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getDeltas(IntPtr output);
    // other
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getTetMeshTransforms(IntPtr translation, IntPtr rotation);
    [DllImport("PlasticDeformationDll")]
    private static extern float dll_getSolverDeltaTime();
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_toggleLoggingOn();
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_toggleLoggingOff();

    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getInitExecutionTimes(IntPtr rotation);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getSolverExecutionTimes(IntPtr rotation);
    /// calculations
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_solve();
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_project(int colliderId);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_solveConstraints();
    ///setup/setdown
    [DllImport("PlasticDeformationDll")]
    private static extern bool dll_init();
    [DllImport("PlasticDeformationDll")]
    private static extern bool dll_init_S();
    [DllImport("PlasticDeformationDll")]
    private static extern bool dll_initFromFile(string fileName, int length);
    [DllImport("PlasticDeformationDll")]
    private static extern bool dll_storeData();
    [DllImport("PlasticDeformationDll")]
    private static extern bool dll_storeData(string fileName, int length);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_setFileName(string name, int length);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_setFilePath(string path, int length);
    [DllImport("PlasticDeformationDll")]
    private static extern bool dll_readTetMeshFiles(string fileName);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_setSurfaceVertices(Vector3[] surfaceVertices, int surfVertCount);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_setTetMeshData(Vector3[] vertices, int vertCount, int[] tetrahedra, int tetCount);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_setColliders(Vector3[] colliderPositions, Vector3[] colliderSizes, ColliderType[] colliderTypes, int colliderCount);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_setTetMeshTransforms(Vector3 translation, Vector3 rotation);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_setIterationCount(int iterationCount);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_setPlasticity(float plasticity);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_setCollisions(int[] collisions, int collisionCount);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_teardown();
    /// tests
    [DllImport("PlasticDeformationDll")]
    private static extern int dll_getDebugInt();
    [DllImport("PlasticDeformationDll")]
    private static extern float dll_getDebugFloat();
    
    #endregion region DLL definition

    private void Awake() {
        singleton = this;
        collisions = new List<int>();
        if(useInitFromFile)
            initializeDLLFromFile();
        else
            initializeDLL();
    }

    void initializeDLLFromFile() {
        dll_setFileName(fileName, fileName.Length);
        dll_setFilePath(filePath, filePath.Length);
        // enable logging
        dll_toggleLoggingOn();
        // set up solver
        dll_setIterationCount((int)iterationSlider.value);
        dll_setPlasticity(plasticitySlider.value);
        string tetmeshFileName = fileName+".tetMesh";
        if (dll_initFromFile(tetmeshFileName, tetmeshFileName.Length)) {
            tetMesh.updateCarModel(getSurfaceVerticesFromDll());
            Debug.Log("DLL Initialized!");
            startSimulation();
        } else {
            Debug.Log("ERROR: DLL Initialization failed!");
        }
    }   

    void initializeDLL() {
        // set up mesh data
        dll_setFileName(fileName, fileName.Length);
        dll_setFilePath(filePath, filePath.Length);
        // enable logging
        dll_toggleLoggingOn();
        // set surface mesh data
        dll_setSurfaceVertices(tetMesh.getSurfaceVertices(), tetMesh.getSurfaceVertices().Length);
        // set up solver
        dll_setIterationCount((int)iterationSlider.value);
        dll_setPlasticity(plasticitySlider.value);
        // intialize dll side and start simulation
        bool isInitSuccess = false;
        if (useSerialInit)
            isInitSuccess = dll_init_S();
        else
            isInitSuccess = dll_init();

        if (isInitSuccess) {
            if(useStoreData)
                dll_storeData();
            // tetMesh.setTetMeshSurface(getTetMeshSurfaceVerticesFromDll(), getTetMeshSurfaceTrianglesFromDll());
            tetMesh.updateCarModel(getSurfaceVerticesFromDll());
            Debug.Log("DLL Initialized!");
            startSimulation();
        } else {
            Debug.Log("ERROR: DLL Initialization failed!");
        }

    }

    void Update () {  
        if (isSimulating) {
            // update transforms of the tet mesh
            Vector3 translation, rotation;
            tetMesh.getTransforms(out translation, out rotation);
            dll_setTetMeshTransforms(translation, rotation);
            // set current collisions
            dll_setCollisions(collisions.ToArray(), collisions.Count);
            collisions.Clear();
            // call dll solve
            dll_solve();
            outputCollisionInfo();
            outputSolverDeltaTime();
            tetMesh.updateCarModel(getSurfaceVerticesFromDll());
           // tetMesh.updateSurfaceModel(getTetMeshSurfaceVerticesFromDll());
        }
	}

    private void OnDestroy() {
        dll_teardown();
    }

    public void addCollision(int id) {
        collisions.Add(id);
    }

    private void setCollisions() {
    }

    // Passes surface vertices to the dll.
    public void setSurfaceData(Vector3[] surfaceVerts) {
        surfVertCount = surfaceVerts.Length;
        dll_setSurfaceVertices(surfaceVerts, surfaceVerts.Length);
    }

    public void solveConstraints() {
        dll_solveConstraints();
        outputCollisionInfo();
        outputSolverDeltaTime();
      //  tetMesh.updateSurfaceModel(getTetMeshSurfaceVerticesFromDll());
    }

    public void projectCollision(int collId) {
        dll_project(collId);
        outputCollisionInfo();
        outputSolverDeltaTime();
      //  tetMesh.updateSurfaceModel(getTetMeshSurfaceVerticesFromDll());
    }

    public void startSimulation() {
        isSimulating = true;
        logExecutionTimes();
    }

    private void logExecutionTimes() {
        int[] times = getInitExecutionTimesFromDll();
        string text = "Init total " + times[0]
            + " ms, read file: " + times[1]
            + " ms, generate constraints: " + times[2]
            + " ms, bc find tet ids: " + times[3]
            + " ms, bc mapping: " + times[4]
            + " ms, serialized save: " + times[5]
            + " ms, serialized load: " + times[6]+"\n";
        Debug.Log(text);
        string path = @"F:\Eigene Dateien\Studium\Master Schweden\9 Master Thesis\Practical\MasterThesisPrototype\Logs\times.log";
        File.AppendAllText(path, text);
    }

    #region setters
    public void setPlasticity(float plasticity) {
        dll_setPlasticity(plasticity);
    }

    public void setIteration(float iterationCount) {
        dll_setIterationCount((int)iterationCount);
    }

    public void setupColliders() {
        Vector3[] collPositions, collSizes;
        ColliderType[] collTypes;
        int colliderCount;
        ColliderManager.getColliderData(out collPositions, out collSizes, out collTypes, out colliderCount);
        dll_setColliders(collPositions, collSizes, collTypes, colliderCount);
    }
    #endregion setters
    #region getters
    public Vector4[] getBarycentricCoordinatesFromDll() {
        Vector4[] resultArray = new Vector4[dll_getBarycentricCoordCount()];
        GCHandle arrHandle = GCHandle.Alloc(resultArray, GCHandleType.Pinned);
        IntPtr arrPtr = arrHandle.AddrOfPinnedObject();
        dll_getBarycentricCoordinates(arrPtr);
        return resultArray;
    }

    public int[] getBarycentricTetIdFromDll() {
        int[] resultArray = new int[dll_getBarycentricTetIdCount()];
        GCHandle arrHandle = GCHandle.Alloc(resultArray, GCHandleType.Pinned);
        IntPtr arrPtr = arrHandle.AddrOfPinnedObject();
        dll_getBarycentricTetIds(arrPtr);
        return resultArray;
    }

    public float[] getDeltasFromDll() {
        float[] resultArray = new float[dll_getDeltasCount()];
        GCHandle arrHandle = GCHandle.Alloc(resultArray, GCHandleType.Pinned);
        IntPtr arrPtr = arrHandle.AddrOfPinnedObject();
        dll_getDeltas(arrPtr);
        return resultArray;
    }

    public int[] getInitExecutionTimesFromDll() {
        int[] resultArray = new int[7];
        GCHandle arrHandle = GCHandle.Alloc(resultArray, GCHandleType.Pinned);
        IntPtr arrPtr = arrHandle.AddrOfPinnedObject();
        dll_getInitExecutionTimes(arrPtr);
        return resultArray;
    }

    public int[] getSolverExecutionTimesFromDll() {
        int[] resultArray = new int[13];
        GCHandle arrHandle = GCHandle.Alloc(resultArray, GCHandleType.Pinned);
        IntPtr arrPtr = arrHandle.AddrOfPinnedObject();
        dll_getSolverExecutionTimes(arrPtr);
        return resultArray;
    }

    public Vector3[] getVerticesFromDll() {
        Vector3[] resultArray = new Vector3[dll_getVertexCount()];
        GCHandle arrHandle = GCHandle.Alloc(resultArray, GCHandleType.Pinned);
        IntPtr arrPtr = arrHandle.AddrOfPinnedObject();
        dll_getVertices(arrPtr);
        return resultArray;
    }
    public int[] getTetrahedraFromDll() {
        int[] resultArray = new int[dll_getTetrahedronCount()*4];
        GCHandle arrHandle = GCHandle.Alloc(resultArray, GCHandleType.Pinned);
        IntPtr arrPtr = arrHandle.AddrOfPinnedObject();
        dll_getVertices(arrPtr);
        return resultArray;
    }

    public Vector3[] getSurfaceVerticesFromDll() {
        Vector3[] resultArray = new Vector3[dll_getSurfaceVertexCount()];
        GCHandle arrHandle = GCHandle.Alloc(resultArray, GCHandleType.Pinned);
        IntPtr arrPtr = arrHandle.AddrOfPinnedObject();
        dll_getSurfaceVertices(arrPtr);
        return resultArray;
    }

    /*public Vector3[] getTetMeshSurfaceVerticesFromDll() {
        Vector3[] resultArray = new Vector3[dll_getTetMeshSurfaceVertexCount()];
        GCHandle arrHandle = GCHandle.Alloc(resultArray, GCHandleType.Pinned);
        IntPtr arrPtr = arrHandle.AddrOfPinnedObject();
        dll_getTetMeshSurfaceVertices(arrPtr);
        return resultArray;
    }*/
/*
    public int[] getTetMeshSurfaceTrianglesFromDll() {
        int[] result = new int[dll_getTetMeshSurfaceTriangleCount()*3];
        GCHandle arrHandle = GCHandle.Alloc(result, GCHandleType.Pinned);
        IntPtr arrPtr = arrHandle.AddrOfPinnedObject();
        dll_getTetMeshSurfaceTriangles(arrPtr);
        return result;
    }*/

    public float[] getConstraintRestValuesFromDll() {
        float[] resultArray = new float[dll_getConstraintCount()];
        GCHandle arrHandle = GCHandle.Alloc(resultArray, GCHandleType.Pinned);
        IntPtr arrPtr = arrHandle.AddrOfPinnedObject();
        dll_getConstraintRestValues(arrPtr);
        return resultArray;
    }

    public ColliderData[] getCollidersFromDll() {
        int collCount = ColliderManager.getColliderList().Count;
        // initialize handles and pointers
        Vector3[] collPositions = new Vector3[collCount];
        GCHandle posArrHandle = GCHandle.Alloc(collPositions, GCHandleType.Pinned);
        IntPtr posArrPtr = posArrHandle.AddrOfPinnedObject();

        Vector3[] collSizes = new Vector3[collCount];
        GCHandle sizeArrHandle = GCHandle.Alloc(collSizes, GCHandleType.Pinned);
        IntPtr sizeArrPtr = sizeArrHandle.AddrOfPinnedObject();

        ColliderType[] collTypes = new ColliderType[collCount];
        GCHandle typeArrHandle = GCHandle.Alloc(collTypes, GCHandleType.Pinned);
        IntPtr typeArrPtr = typeArrHandle.AddrOfPinnedObject();
        //retrieve data from dll
        dll_getColliders(posArrPtr, sizeArrPtr, typeArrPtr);

        ColliderData[] dllColliders = new ColliderData[collCount];
        ColliderData collData;
        for (int i=0; i< collCount; i++) {
            collData.position = collPositions[i];
            collData.size = collSizes[i];
            collData.type = collTypes[i];
            collData.id = i;

            dllColliders[i] = collData;
        }
        return dllColliders;
    }
    #endregion getters
    #region Logger
    private void logDebugInt() {
        Debug.Log("DebugInt: " + dll_getDebugInt());
    }

    private void logDebugFloat() {
        Debug.Log("DebugFloat: " + dll_getDebugFloat());
    }

    private void logDebugInt(string pretext) {
        Debug.Log(pretext + " DebugInt: " + dll_getDebugInt());
    }

    private void logDebugFloat(string pretext) {
        Debug.Log(pretext + " DebugFloat: " + dll_getDebugFloat());
    }

    private void outputSolverDeltaTime() {
        solverDeltaTimeText.text = "Solver Delta Time: " + dll_getSolverDeltaTime()+ " ms";
    }
    private void outputCollisionInfo() {
        collisionCountText.text = "collision count: " + dll_getCollisionCount();
    }
    #endregion Logger
}