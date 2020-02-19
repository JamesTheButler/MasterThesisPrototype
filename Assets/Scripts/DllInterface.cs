﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class DllInterface : MonoBehaviour {
    public GameObject infoGO;

    [SerializeField] [Range(1, 10)] private int solverIterationCount;
    [SerializeField] [Range(0, 1)] private float plasticity;
    [SerializeField] private TetrahedralMesh tetMesh;
    [SerializeField] private Text solverDeltaTimeText;
    [SerializeField] private Text collisionCountText;
    [SerializeField] private Transform carTransform;

    [SerializeField] private string filePath;
    [SerializeField] private string backupFilePath;
    [SerializeField] private string fileName;

    private static DllInterface singleton;
    public static DllInterface getSingleton() { return singleton; }

    private int vertCount, tetCount, surfVertCount;
    private bool isSimulating=false;

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
    //tet mesh surface data
    [DllImport("PlasticDeformationDll")]
    private static extern int dll_getTetMeshSurfaceVertexCount();
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getTetMeshSurfaceVertices(IntPtr verts);
    [DllImport("PlasticDeformationDll")]
    private static extern int dll_getTetMeshSurfaceTriangleCount();
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getTetMeshSurfaceTriangles(IntPtr tris);
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
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getTestCurrDist(IntPtr ouput);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getTestNewRest(IntPtr ouput);
    // other
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getTetMeshTransforms(IntPtr translation, IntPtr rotation);
    [DllImport("PlasticDeformationDll")]
    private static extern float dll_getSolverDeltaTime();
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_toggleLoggingOn();
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_toggleLoggingOff();
    /// calculations
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getCollisionResult(int colliderId);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_project(int colliderId);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_solveConstraints();
    ///setup/setdown
    [DllImport("PlasticDeformationDll")]
    private static extern bool dll_init();
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
    private static extern void dll_teardown();
    /// tests
    [DllImport("PlasticDeformationDll")]
    private static extern int dll_getDebugInt();
    [DllImport("PlasticDeformationDll")]
    private static extern float dll_getDebugFloat();
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_testVectorRotation(IntPtr outputRotated, IntPtr outputUnrotated, IntPtr baseRotArrPtr, Vector3 vector, Vector3 rotation);
    [DllImport("PlasticDeformationDll")]
    private static extern bool dll_testVertexAABoxIntersection(Vector3 vertex, Vector3 cPos, Vector3 cSize);
    [DllImport("PlasticDeformationDll")]
    private static extern bool dll_testEqualityVertices(Vector3[] vertices, int vertCount);
    [DllImport("PlasticDeformationDll")]
    private static extern bool dll_testEqualityTetrahedra(int[] tets, int tetCount);
    [DllImport("PlasticDeformationDll")]
    private static extern bool dll_testEqualitySurfaceVertexIndeces(int[] surfVerts, int vertCount);
    [DllImport("PlasticDeformationDll")]
    private static extern bool dll_testEqualitySurfaceTriangles(int[] tris, int triCount);
    [DllImport("PlasticDeformationDll")]
    private static extern bool dll_testEqualityConstraints(int[] vertexIds, int vertexIdCount, float[] currentValues, float[] restValues, EConstraintType[] constraintTypes, int constraintCount);
    [DllImport("PlasticDeformationDll")]
    private static extern int dll_getConstraintPerVertexCount(int id);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getConstraintPerVertex(int id, IntPtr output);
    #endregion region DLL definition

    private void Awake() {
        singleton = this;
        // set up mesh data
        dll_setFileName(fileName, fileName.Length);
        dll_setFilePath(filePath, filePath.Length);
        // enable logging
        dll_toggleLoggingOn();
        // set surface mesh data
        Debug.Log(tetMesh.getSurfaceVertices().Length);
        dll_setSurfaceVertices(tetMesh.getSurfaceVertices(), tetMesh.getSurfaceVertices().Length);
        // set up solver
        dll_setIterationCount(solverIterationCount);
        dll_setPlasticity(plasticity);
        // intialize dll side and start simulation
        if (dll_init()) {
            tetMesh.setTetMeshSurface(getTetMeshSurfaceVerticesFromDll(), getTetMeshSurfaceTrianglesFromDll());
            //tetMesh.updateCarModel(getSurfaceVerticesFromDll());
            Debug.Log(getVerticesFromDll().Length);
            //Debug.Log(getSurfaceVerticesFromDll().Length);
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
        }
	}

    private void OnDestroy() {
        dll_teardown();
    }

    public void getCollisionResult(int colliderId) {
        if (false/*isSimulating*/) {
            dll_getCollisionResult(colliderId);
            outputCollisionInfo();
            outputSolverDeltaTime();
            tetMesh.updateCarModel(getSurfaceVerticesFromDll());
            tetMesh.updateSurfaceModel(getTetMeshSurfaceVerticesFromDll());
        }
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
        tetMesh.updateSurfaceModel(getTetMeshSurfaceVerticesFromDll());
    }

    public void projectCollision(int collId) {
        dll_project(collId);
        outputCollisionInfo();
        outputSolverDeltaTime();
        tetMesh.updateSurfaceModel(getTetMeshSurfaceVerticesFromDll());
    }

    public void startSimulation() {
        isSimulating = true;
        Debug.Log("Start-Up took " + Time.realtimeSinceStartup + " sec");
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
        MyColliderManager.getColliderData(out collPositions, out collSizes, out collTypes);
        dll_setColliders(collPositions, collSizes, collTypes, collPositions.Length);
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

    public int[] getConstraintPerVertexFromDll(int vertId) {
        int[] resultArray = new int[dll_getConstraintPerVertexCount(vertId)];
        GCHandle arrHandle = GCHandle.Alloc(resultArray, GCHandleType.Pinned);
        IntPtr arrPtr = arrHandle.AddrOfPinnedObject();
        dll_getConstraintPerVertex(vertId, arrPtr);
        return resultArray;
    }

    public float[] getTestCurrDistsFromDll() {
        float[] resultArray = new float[dll_getDeltasCount()];
        GCHandle arrHandle = GCHandle.Alloc(resultArray, GCHandleType.Pinned);
        IntPtr arrPtr = arrHandle.AddrOfPinnedObject();
        dll_getTestCurrDist(arrPtr);
        return resultArray;
    }
    public float[] getTestNewRestsFromDll() {
        float[] resultArray = new float[dll_getDeltasCount()];
        GCHandle arrHandle = GCHandle.Alloc(resultArray, GCHandleType.Pinned);
        IntPtr arrPtr = arrHandle.AddrOfPinnedObject();
        dll_getTestNewRest(arrPtr);
        return resultArray;
    }

    public float[] getDeltasFromDll() {
        float[] resultArray = new float[dll_getDeltasCount()];
        GCHandle arrHandle = GCHandle.Alloc(resultArray, GCHandleType.Pinned);
        IntPtr arrPtr = arrHandle.AddrOfPinnedObject();
        dll_getDeltas(arrPtr);
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

    public Vector3[] getTetMeshSurfaceVerticesFromDll() {
        Vector3[] resultArray = new Vector3[dll_getTetMeshSurfaceVertexCount()];
        GCHandle arrHandle = GCHandle.Alloc(resultArray, GCHandleType.Pinned);
        IntPtr arrPtr = arrHandle.AddrOfPinnedObject();
        dll_getTetMeshSurfaceVertices(arrPtr);
        return resultArray;
    }

    public int[] getTetMeshSurfaceTrianglesFromDll() {
        int[] result = new int[dll_getTetMeshSurfaceTriangleCount()*3];
        GCHandle arrHandle = GCHandle.Alloc(result, GCHandleType.Pinned);
        IntPtr arrPtr = arrHandle.AddrOfPinnedObject();
        dll_getTetMeshSurfaceTriangles(arrPtr);
        return result;
    }

    public float[] getConstraintRestValuesFromDll() {
        float[] resultArray = new float[dll_getConstraintCount()];
        GCHandle arrHandle = GCHandle.Alloc(resultArray, GCHandleType.Pinned);
        IntPtr arrPtr = arrHandle.AddrOfPinnedObject();
        dll_getConstraintRestValues(arrPtr);
        return resultArray;
    }

    public MyColliderData[] getCollidersFromDll() {
        int collCount = MyColliderManager.getColliderList().Count;
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

        MyColliderData[] dllColliders = new MyColliderData[collCount];
        MyColliderData collData;
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