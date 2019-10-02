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

    private bool isReadyForCollisionChecks;

    /// DLL Methods
    #region DLL definition
    // setters
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_setVertices(Vector3[] verts, int vertexCount);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_setColliders(Vector3[] colliderPositions, Vector3[] colliderSizes, ColliderType[] colliderTypes, int colliderCount);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_setConstraints(  int[] vertexIds, int[] startingVertexIdArrayIndeces, int[] vertexIdArrayLengths, 
                                                    float[] currentValue, float[] restValue, ConstraintType[] type, int constraintCount);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_setTetMeshTransforms(Vector3 translation, Vector3 rotation);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_setIterationCount(int iterationCount);

    //getters
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getVertices(IntPtr verts);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getColliders(IntPtr positions, IntPtr sizes, IntPtr types);
    [DllImport("PlasticDeformationDll")]
    private static extern int dll_getCollisionCount();
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getTetMeshTransforms(IntPtr translation, IntPtr rotation);

    // calculations
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getCollisionResult(int colliderId);

    //setup/setdown
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_init();
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_teardown();

    // tests
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_testVectorRotation(IntPtr outputRotated, IntPtr outputUnrotated, IntPtr baseRotArrPtr, Vector3 vector, Vector3 rotation);
    [DllImport("PlasticDeformationDll")]
    private static extern bool dll_testVertexAABoxIntersection(Vector3 vertex, Vector3 cPos, Vector3 cSize);
    [DllImport("PlasticDeformationDll")]
    private static extern int dll_getDebugInt();
    [DllImport("PlasticDeformationDll")]
    private static extern float dll_getDebugFloat();

    #endregion region DLL definition

    private void Start() {
        singleton = this;
        dll_init();
        //dll_setIterationCount(solverIterationCount);

        //testPointAABoxIntersection();
        //testRotationCalculation(new Vector3(22, -35, -21), new Vector3(340, -23, 400));
    }

    public void init() {
        initData();
        setReadyForCollisionChecks(true);
        //logDebugInt();
    }

    private void initData() {
        setVertices();
        setColliders();
        setConstraints();
    }

    private void logDebugInt() {
        Debug.Log("DebugInt: " + dll_getDebugInt());
    }

    private void logDebugFloat() {
        Debug.Log("DebugFloat: " + dll_getDebugFloat());
    }

    private static void testPointAABoxIntersection() {
        Debug.Log(dll_testVertexAABoxIntersection(new Vector3(1, 2.6f, 3), new Vector3(1, 1, 1), new Vector3(2, 3, 4)));
        Debug.Log(dll_testVertexAABoxIntersection(new Vector3(1, 2.5f, 3), new Vector3(1, 1, 1), new Vector3(2, 3, 4)));
    }

    void Update () {  
        if (isReadyForCollisionChecks) {
            // dll input
            Vector3 translation, rotation;
            tetMesh.getTransforms(out translation, out rotation);
            dll_setTetMeshTransforms(translation, rotation);
            // solving
            /*dll_solve();
            // dll output
            tetMesh.updateVertices(getVerticesFromDll());
            getCollisionResult();*/
        }
	}

    private void OnDestroy() {
        dll_teardown();
    }


    private void outputCollisionInfo() {
        collisionCountText.text = "collision count: " + dll_getCollisionCount();
    }

    public void getCollisionResult(int colliderId) {
        if (isReadyForCollisionChecks) {
            dll_getCollisionResult(colliderId);
            outputCollisionInfo();
        }
    }

    #region setters
    public void setReadyForCollisionChecks(bool isReady) {
        isReadyForCollisionChecks = isReady;
    }

    private void setVertices() {
        Vector3[] vertices = singleton.tetMesh.getVertices();
        dll_setVertices(vertices, vertices.Length);
    }

    private void setColliders() {
        Vector3[] collPositions, collSizes;
        ColliderType[] collTypes;
        MyColliderManager.getColliderData(out collPositions, out collSizes, out collTypes);
        dll_setColliders(collPositions, collSizes, collTypes, collPositions.Length);

        logDebugFloat();
    }

    private void setConstraints() {
        List<int> vertexIds = new List<int>();
        List<int> startingIndeces = new List<int>();
        List<int> vertexIdArrayLengths = new List<int>();
        List<float> currentValues = new List<float>();
        List<float> restValues = new List<float>();
        List<ConstraintType> constraintTypes = new List<ConstraintType>();
        int constraintCount = 0;

        // set up constraint info lists
        List<Constraint> constraints = tetMesh.getConstraints();
        constraintCount = constraints.Count;
        foreach(Constraint constraint in constraints) {
            int[] verts = constraint.getVertices();
            int length = verts.Length;
            //set starting index
            startingIndeces.Add(vertexIds.Count);
            //set length
            vertexIdArrayLengths.Add(length);
            //set verts
            for (int i=0; i<length; i++) {
                vertexIds.Add(verts[i]);
            }
            currentValues.Add(constraint.getCurrentValue());
            restValues.Add(constraint.getRestValue());
            constraintTypes.Add(constraint.getConstraintType());
        }
        dll_setConstraints(vertexIds.ToArray(), startingIndeces.ToArray(), vertexIdArrayLengths.ToArray(), currentValues.ToArray(), restValues.ToArray(), constraintTypes.ToArray(), constraintCount);
    }
    #endregion setters

    #region getters
    public void testRotationCalculation(Vector3 vector, Vector3 rotation) {
        Vector3[] rotated = new Vector3[1];
        GCHandle rotatedArrHandle = GCHandle.Alloc(rotated, GCHandleType.Pinned);
        IntPtr rotatedArrPtr = rotatedArrHandle.AddrOfPinnedObject();

        Vector3[] unrotated = new Vector3[1];
        GCHandle unrotatedArrHandle = GCHandle.Alloc(unrotated, GCHandleType.Pinned);
        IntPtr unrotatedArrPtr = unrotatedArrHandle.AddrOfPinnedObject();

        Vector3[] baseRot = new Vector3[1];
        GCHandle baseRotArrHandle = GCHandle.Alloc(baseRot, GCHandleType.Pinned);
        IntPtr baseRotArrPtr = baseRotArrHandle.AddrOfPinnedObject();

        dll_testVectorRotation(rotatedArrPtr, unrotatedArrPtr, baseRotArrPtr, vector, rotation);

        Debug.Log("rotated vector" + vector + " by " + baseRot[0] + "... rotated result: " + rotated[0] + "... unrotated result: " + unrotated[0]);
        // check for correct data
        infoGO.GetComponent<Info>().getResult(vector, rotation);
    }

    public Vector3[] getVerticesFromDll() {
        Vector3[] resultArray = new Vector3[singleton.tetMesh.getVertices().Length];
        GCHandle arrHandle = GCHandle.Alloc(resultArray, GCHandleType.Pinned);
        IntPtr arrPtr = arrHandle.AddrOfPinnedObject();
        dll_getVertices(arrPtr);
        return resultArray;
    }

    public MyColliderData[] getCollidersFromDll() {
        int collCount = MyColliderManager.getColliderList().Count;
        MyColliderData[] dllColliders = new MyColliderData[collCount];
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
}