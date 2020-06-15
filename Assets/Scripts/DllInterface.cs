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
    [SerializeField] private Slider distanceStiffnessSlider;
    [SerializeField] private Slider volumeStiffnessSlider;
    [SerializeField] private TetrahedralMesh tetMesh;
    [SerializeField] private Text solverDeltaTimeText;
    [SerializeField] private Text collisionCountText;
    [SerializeField] private Transform carTransform;

    [SerializeField] private string filePath;
    [SerializeField] private string backupFilePath;
    [SerializeField] private string fileName;
    private string storeFileName;
       
    [SerializeField] private bool useSerialInit = false;
    [SerializeField] private bool useSurfaceToTetMap = false;
    [SerializeField] private bool useInitFromFile = false;
    [SerializeField] private bool useStoreData = false;
    [SerializeField] private bool useSerialLoop = false;
   

    private static DllInterface singleton;
    public static DllInterface getSingleton() { return singleton; }

    private int vertCount, tetCount, surfVertCount;
    private bool isSimulating=false;

    private string tetFilePath;

    private List<int> collisions;

    private List<int[]> solverTimes;

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
    private static extern void dll_getSurfaceVertices_m(IntPtr verts);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getSurfaceVertices_s(IntPtr verts);
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
    private static extern void dll_solve_s();
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_project(int colliderId);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_solveConstraints();
    ///setup/setdown
    [DllImport("PlasticDeformationDll")]
    private static extern bool dll_init();
    [DllImport("PlasticDeformationDll")]
    private static extern bool dll_init_m();
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
    private static extern void dll_setStoreFileName(string name, int length);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_setFilePath(string path, int length);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_setTetrahedralizationPath(string path, int length);
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
    private static extern void dll_setDistanceStiffness(float stiffness);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_setVolumeStiffness(float stiffness);    
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
        solverTimes = new List<int[]>();
        
        storeFileName = fileName;
        if (useSurfaceToTetMap)
            storeFileName += "s2t";
        else if (useSerialLoop)
            storeFileName += "serial";

        tetFilePath = filePath + "Tetrahedralization/PerformanceTests/371/";

        if (useInitFromFile)
            initializeDLLFromFile();
        else
            initializeDLL();
    }


    void initializeDLLFromFile() {
        dll_setFileName(fileName, fileName.Length);
        dll_setStoreFileName(storeFileName, storeFileName.Length);
        dll_setFilePath(filePath, filePath.Length);
        Debug.Log(tetFilePath);
        dll_setTetrahedralizationPath(tetFilePath, tetFilePath.Length);
        // enable logging
        dll_toggleLoggingOn();
        // set up solver
        dll_setIterationCount((int)iterationSlider.value);
        dll_setPlasticity(plasticitySlider.value);
        dll_setDistanceStiffness(distanceStiffnessSlider.value);
        dll_setVolumeStiffness(volumeStiffnessSlider.value);
        string tetmeshFileName = storeFileName+".tetMesh";
        if (dll_initFromFile(tetmeshFileName, tetmeshFileName.Length)) {
            tetMesh.updateCarModel(getSurfaceVerticesFromDll());
            Debug.Log("float "+dll_getDebugFloat());
            Debug.Log("int "+dll_getDebugInt());
            Debug.Log("DLL Initialized!");
            startSimulation();
        } else {
            Debug.Log("ERROR: DLL Initialization failed!");
        }
    }

    void initializeDLL() {
        // set up mesh data
        dll_setFileName(fileName, fileName.Length);
        dll_setStoreFileName(storeFileName, storeFileName.Length);
        dll_setFilePath(filePath, filePath.Length);
        Debug.Log(tetFilePath);
        dll_setTetrahedralizationPath(tetFilePath, tetFilePath.Length);
        // enable logging
        dll_toggleLoggingOn();
        // set surface mesh data
        dll_setSurfaceVertices(tetMesh.getSurfaceVertices(), tetMesh.getSurfaceVertices().Length);
        // set up solver
        dll_setIterationCount((int)iterationSlider.value);
        dll_setPlasticity(plasticitySlider.value);
        dll_setDistanceStiffness(distanceStiffnessSlider.value);
        dll_setVolumeStiffness(volumeStiffnessSlider.value);
        // intialize dll side and start simulation
        bool isInitSuccess = false;
        if (useSerialInit) {
            Debug.Log("Serial Init");
            isInitSuccess = dll_init_S();
        } else if (useSurfaceToTetMap) {
            Debug.Log("Parallel Init with Map");
            isInitSuccess = dll_init_m();
        } else {
            Debug.Log("Parallel Init");
            isInitSuccess = dll_init();
        }
        if (isInitSuccess) {
            if (useStoreData) {
                Debug.Log("Store Data");
                dll_storeData();
            }
            // tetMesh.setTetMeshSurface(getTetMeshSurfaceVerticesFromDll(), getTetMeshSurfaceTrianglesFromDll());
            if (useSurfaceToTetMap) {
                tetMesh.updateCarModel(getSurfaceVerticesFromDll_m());
            } else {
                tetMesh.updateCarModel(getSurfaceVerticesFromDll());
            }

            Debug.Log("DLL Initialized");
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
            if (useSerialLoop) {
                dll_solve_s();
                //Debug.Log("serial loop");
            } else {
                dll_solve();
            }
            outputCollisionInfo();
            outputSolverDeltaTime();

            if (useSurfaceToTetMap) {
                tetMesh.updateCarModel(getSurfaceVerticesFromDll_m());
            } else {
                if (useSerialLoop) {
                    tetMesh.updateCarModel(getSurfaceVerticesFromDll_s());
                } else {
                    tetMesh.updateCarModel(getSurfaceVerticesFromDll());
                }
            }
            List<int> temp = new List<int>(getSolverExecutionTimesFromDll());
            temp.Add((int)iterationSlider.value);
            solverTimes.Add(temp.ToArray());
        }
	}

    private void OnDestroy() {
        logSolverTimes();
        dll_teardown();
    }

    public void addCollision(int id) {
        collisions.Add(id);
    }

    // Passes surface vertices to the dll.
    public void setSurfaceData(Vector3[] surfaceVerts) {
        surfVertCount = surfaceVerts.Length;
        dll_setSurfaceVertices(surfaceVerts, surfaceVerts.Length);
    }

    public void startSimulation() {
        isSimulating = true;
        //logInitTimes();
    }

    private void logSolverTimes() {
        string path = filePath + @"\Logs\Solver\"+storeFileName+".log";
        string text="";
        File.AppendAllText(path,"");
        foreach (int[] timepoint in solverTimes) {
            if (timepoint[0] != 0){
                text = timepoint[0]       // total
                + " " + timepoint[1]            // collision proj
                + " " + timepoint[2]            // constraint solving
                + " " + timepoint[3]            // bc update
                //+ " " + timepoint[4]       // iteration count
                + "\n";    
            }
            File.AppendAllText(path, text);
        }
    }

    private void logInitTimes() {
        int[] times = getInitExecutionTimesFromDll();
        string text = times[0]      // total
            + " " + times[1]        // read file
            + " " + times[2]        // constraint gen
            + " " + times[3]        // find tet ids
            + " " + times[4]        // bcc mapping
            + " " + times[5]        // save
            + " " + times[6]+"\n";  // load
        //Debug.Log(text);
        string path = filePath+ @"\Logs\inittimes.log";
        File.AppendAllText(path, text);
    }

    #region setters
    public void setPlasticity(float plasticity) {
        dll_setPlasticity(plasticity);
    }

    public void setDistanceStiffness(float stiffness) {
        dll_setDistanceStiffness(stiffness);
    }

    public void setVolumeStiffness(float stiffness) {
        dll_setVolumeStiffness(stiffness);
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
        int[] resultArray = new int[4];
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


    public Vector3[] getSurfaceVerticesFromDll_m() {
        Vector3[] resultArray = new Vector3[dll_getSurfaceVertexCount()];
        GCHandle arrHandle = GCHandle.Alloc(resultArray, GCHandleType.Pinned);
        IntPtr arrPtr = arrHandle.AddrOfPinnedObject();
        dll_getSurfaceVertices_m(arrPtr);
        return resultArray;
    }


    public Vector3[] getSurfaceVerticesFromDll() {
        Vector3[] resultArray = new Vector3[dll_getSurfaceVertexCount()];
        GCHandle arrHandle = GCHandle.Alloc(resultArray, GCHandleType.Pinned);
        IntPtr arrPtr = arrHandle.AddrOfPinnedObject();
        dll_getSurfaceVertices(arrPtr);
        return resultArray;
    }

    public Vector3[] getSurfaceVerticesFromDll_s() {
        Vector3[] resultArray = new Vector3[dll_getSurfaceVertexCount()];
        GCHandle arrHandle = GCHandle.Alloc(resultArray, GCHandleType.Pinned);
        IntPtr arrPtr = arrHandle.AddrOfPinnedObject();
        dll_getSurfaceVertices_s(arrPtr);
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