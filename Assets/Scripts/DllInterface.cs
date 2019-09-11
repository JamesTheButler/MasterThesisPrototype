using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class DllInterface : MonoBehaviour {
    [SerializeField] private TetrahedralMesh tetMesh;
    [SerializeField] private Text collisionCountText;
    [SerializeField] private MeshBuilder meshBuilder;
    [SerializeField] private ColliderSetter collSetter;

    private static DllInterface singleton;
    public static DllInterface getSingleton() { return singleton; }

    private bool isReadyForCollisionChecks;

    /// DLL Methods
    // setters
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_setVertices(Vector3[] verts, int vertexCount);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_setColliders(Vector3[] colliderPositions, Vector3[] colliderSizes, ColliderType[] colliderTypes, int colliderCount);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_setConstraints(  int[] vertexIds, int[] startingVertexIdArrayIndeces, int[] vertexIdArrayLengths, 
                                                    float[] currentValue, float[] restValue, ConstraintType[] type, int constraintCount);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_setTetMeshTransforms(Vector3 translation, Quaternion rotation);
    //getters
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getColliders(IntPtr positions, IntPtr sizes, IntPtr types);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getVertices(IntPtr verts);
    [DllImport("PlasticDeformationDll")]
    private static extern int dll_getCollisionCount();
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getTetMeshTransforms(IntPtr translation, IntPtr rotation);
    // calculations
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_collisionHandling();

    [DllImport("PlasticDeformationDll")]
    private static extern void dll_teardown();

    private void Start() {
        singleton = this;
    }

    void Update () {
        if (isReadyForCollisionChecks) {
            Vector3 translation;
            Quaternion rotation;
            tetMesh.getTransforms(out translation, out rotation);
            dll_setTetMeshTransforms(translation, rotation);
            getCollisionResult();

            if (!collSetter.isSet)
                collSetter.setInfo(getCollidersFromDll());
            meshBuilder.setVertexData(getVerticesFromDll());
            //meshBuilder.setTransforms(translation, rotation);
        }
	}

    private void OnDestroy() {
        dll_teardown();
    }

    public void setReadyForCollisionChecks(bool isReady) {
        isReadyForCollisionChecks = isReady;
    }

    public void initData() {
        updateVertices();
        updateColliders();
        updateConstraints();
    }

    private void updateVertices() {
        Vector3[] vertices = singleton.tetMesh.getVertices();
        dll_setVertices(vertices, vertices.Length);
    }

    private void updateColliders() {
        Vector3[] collPos, collSizes;
        ColliderType[] collTypes;
        ColliderManager.getColliderData(out collPos, out collSizes, out collTypes);
        dll_setColliders(collPos, collSizes, collTypes, collPos.Length);
    }

    private void updateConstraints() {
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
    
    public void getCollisionResult() {
        dll_collisionHandling();
        int collisionCount = dll_getCollisionCount();
        collisionCountText.text = "Collision Count: " + collisionCount;
    }


    public Vector3[] getVerticesFromDll() {
        Vector3[] resultArray = new Vector3[singleton.tetMesh.getVertices().Length];
        GCHandle arrHandle = GCHandle.Alloc(resultArray, GCHandleType.Pinned);
        IntPtr arrPtr = arrHandle.AddrOfPinnedObject();
        dll_getVertices(arrPtr);
        return resultArray;
    }

    public MyColliderData[] getCollidersFromDll() {
        int collCount = ColliderManager.getColliderList().Count;
        MyColliderData[] dllColliders = new MyColliderData[collCount];
        // initialize handles and pointers
        GCHandle posArrHandle;
        IntPtr posArrPtr;
        Vector3[] collPositions = new Vector3[collCount];
        posArrHandle = GCHandle.Alloc(collPositions, GCHandleType.Pinned);
        posArrPtr = posArrHandle.AddrOfPinnedObject();

        GCHandle sizeArrHandle;
        IntPtr sizeArrPtr;
        Vector3[] collSizes = new Vector3[collCount];
        sizeArrHandle = GCHandle.Alloc(collSizes, GCHandleType.Pinned);
        sizeArrPtr = sizeArrHandle.AddrOfPinnedObject();

        GCHandle typeArrHandle;
        IntPtr typeArrPtr;
        ColliderType[] collTypes = new ColliderType[collCount];
        typeArrHandle = GCHandle.Alloc(collTypes, GCHandleType.Pinned);
        typeArrPtr = typeArrHandle.AddrOfPinnedObject();
        //retrieve data from dll
        dll_getColliders(posArrPtr, sizeArrPtr, typeArrPtr);

        MyColliderData collData;
        for (int i=0; i< collCount; i++) {
            collData.position = collPositions[i];
            collData.size = collSizes[i];
            collData.type = collTypes[i];

            dllColliders[i] = collData;
        }
        return dllColliders;
    }

    public void getTetMeshTransformsFromDll(out Vector3 translation, out Quaternion rotation) {
        translation = new Vector3();
        GCHandle posHandle = GCHandle.Alloc(translation, GCHandleType.Pinned);
        IntPtr posPtr = posHandle.AddrOfPinnedObject();

        rotation = new Quaternion();
        GCHandle rotHandle = GCHandle.Alloc(rotation, GCHandleType.Pinned);
        IntPtr rotPtr = rotHandle.AddrOfPinnedObject();

        dll_getTetMeshTransforms(posPtr, rotPtr);
    }
}