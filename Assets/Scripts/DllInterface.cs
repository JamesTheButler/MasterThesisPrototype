using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class DllInterface : MonoBehaviour {
    public GameObject infoGO;

    [SerializeField] private int solverIterationCount;
    [SerializeField] private TetrahedralMesh tetMesh;
    [SerializeField] private Text collisionCountText;

    private static DllInterface singleton;
    public static DllInterface getSingleton() { return singleton; }

    private int _vertCount, _tetCount, _surfVertCount, _surfTriCount;
    private bool isSimulating=false;

    // DLL Methods
#region DLL definition
    //getters
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getVertices(IntPtr verts);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getTempVertices(IntPtr verts);
    [DllImport("PlasticDeformationDll")]
    private static extern int dll_getVertexCount();
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getSurfVertIndeces(IntPtr ids);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getSurfVertices(IntPtr verts);
    [DllImport("PlasticDeformationDll")]
    private static extern int dll_getSurfVertexCount();
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getSurfTriangles(IntPtr tris);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getColliders(IntPtr positions, IntPtr sizes, IntPtr types);
    [DllImport("PlasticDeformationDll")]
    private static extern int dll_getCollisionCount();
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getTetMeshTransforms(IntPtr translation, IntPtr rotation);
    //constraint info
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

    // calculations
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getCollisionResult(int colliderId);

    //setup/setdown
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_init();
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_setTetMeshData(Vector3[] vertices, int vertCount, int[] tetrahedra, int tetCount, 
        Vector3[] surfaceVertices, int surfVertCount, int[] surfaceTriangles, int triCount);
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

    // tests
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
        dll_init();
        dll_setIterationCount(1);
        dll_setPlasticity(0.5f);
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

    public void logArray<T>(T[] array, string pretext, int start, int end) {
        string result = pretext + "";
        result += "len: " + array.Length + "__";
        for (int i = start; i <= end; i++) {
            result += array[i].ToString() + ", ";
        }
        Debug.Log(result);
    }

    public void logArray<T>(T[] array, string pretext, int count ) {
        string result = pretext + "";
        result += "len: " + array.Length + "__";
        for (int i = 0; i < Math.Min(array.Length, count); i++) {
            result += array[i].ToString() + ", ";
        }
        Debug.Log(result);
    }

    public void logArray<T>(T[] array, string pretext) {
        logArray<T>(array, pretext, 10);
    }

    public void logArray<T>(T[] array) {
        logArray<T>(array, "", 10);
    }

    public void getCollisionResult(int colliderId) {
        if (isSimulating) {
            dll_getCollisionResult(colliderId);
            outputCollisionInfo();
            tetMesh.updateSurface(getSurfaceVerticesFromDll());
            /*logArray(getVerticesFromDll(), "Verts: ");

            logArray(getDeltasFromDll(), "Deltas", 100);
            logArray(getTestCurrDistsFromDll(), "Curr Dists: ");
            logArray(getConstraintRestValuesFromDll(), "Old Rests: ");
            logDebugFloat();
            logArray(getTestNewRestsFromDll(), "New Rests: ");
        
            Debug.Log("deltas > 0: " + deltaGT0Count);
            Debug.Log(dll_getConstraintCount());*/
        }
    }

    public void startSimulation() {
        isSimulating = true;
    }

    #region setters
    public void setupColliders() {
        Vector3[] collPositions, collSizes;
        ColliderType[] collTypes;
        MyColliderManager.getColliderData(out collPositions, out collSizes, out collTypes);
        dll_setColliders(collPositions, collSizes, collTypes, collPositions.Length);
    }

    public void setTetMeshData(List<Vector3> vertices, List<int> tetrahedra, List<Vector3> surfaceVertices, List<int> surfaceTriangles) {
        _vertCount = vertices.ToArray().Length;
        _tetCount = tetrahedra.ToArray().Length;
        _surfVertCount = surfaceVertices.ToArray().Length;
        _surfTriCount = surfaceTriangles.ToArray().Length;

        dll_setTetMeshData(vertices.ToArray(), _vertCount, tetrahedra.ToArray(), _tetCount, surfaceVertices.ToArray(), _surfVertCount, surfaceTriangles.ToArray(), _surfTriCount);
        tetMesh.setupSurface(getSurfaceVerticesFromDll(), getSurfaceTrianglesFromDll());

        //logArray(getConstraintPerVertexFromDll(1), "constraints of vertex 1: ");
    }
    #endregion setters
    #region getters
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
        Vector3[] resultArray = new Vector3[_vertCount];
        GCHandle arrHandle = GCHandle.Alloc(resultArray, GCHandleType.Pinned);
        IntPtr arrPtr = arrHandle.AddrOfPinnedObject();
        dll_getVertices(arrPtr);
        return resultArray;
    }

    public Vector3[] getSurfaceVerticesFromDll() {
        Vector3[] resultArray = new Vector3[_surfVertCount];
        GCHandle arrHandle = GCHandle.Alloc(resultArray, GCHandleType.Pinned);
        IntPtr arrPtr = arrHandle.AddrOfPinnedObject();
        dll_getSurfVertices(arrPtr);
        return resultArray;
    }

    public int[] getSurfaceTrianglesFromDll() {
        int[] resultArray = new int[_surfTriCount];
        GCHandle arrHandle = GCHandle.Alloc(resultArray, GCHandleType.Pinned);
        IntPtr arrPtr = arrHandle.AddrOfPinnedObject();
        dll_getSurfTriangles(arrPtr);
        return resultArray;
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

    private void outputCollisionInfo() {
        collisionCountText.text = "collision count: " + dll_getCollisionCount();
    }
    #endregion Logger
}