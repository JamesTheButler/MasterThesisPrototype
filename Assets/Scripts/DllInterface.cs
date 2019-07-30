using System;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class DllInterface : MonoBehaviour {
    [SerializeField] private TetrahedralMesh tetMesh;
    [SerializeField] private Text collisionCountText;
    [SerializeField] private MeshBuilder meshBuilder;

    private static DllInterface singleton;
    public static DllInterface getSingleton() { return singleton; }

    private bool isReadyForCollisionChecks;

    private Vector3 previousPos;
    private Vector3 previousRot;
    //private Vector3 previousScale;

    protected GCHandle arrHandle;
    protected IntPtr arrPtr;

    GCHandle sizeArrHandle;
    IntPtr sizeArrPtr;

    /// DLL Methods
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_setVertices(Vector3[] verts, int vertexCount);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getVertices(IntPtr verts);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_setColliders(Vector3[] colliderPositions, Vector3[] colliderSizes, ColliderType[] colliderTypes, int colliderCount);
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_getColliders(IntPtr positions, IntPtr sizes, IntPtr types);
    [DllImport("PlasticDeformationDll")]
    private static extern int dll_getVertexCountInColliders();
    [DllImport("PlasticDeformationDll")]
    private static extern void dll_setTest(out int resultVerts, out int resultColls, out int resultConstraints);
    [DllImport("PlasticDeformationDll")]
    private static extern int dll_returnCollType();

    private void Start() {
        singleton = this;
    }

    void Update () {
        //physicsCalculations();
        if (isReadyForCollisionChecks) {
            dll_setVertices(tetMesh.getGlobalVertices(), tetMesh.getGlobalVertices().Length);

            collisionCountText.text = "Collision Count: "+dll_getVertexCountInColliders();
            //getCollisionResult();
            meshBuilder.setVertexData(getVerticesFromDll());
        }
	}

    public void setReadyForCollisionChecks(bool isReady) {
        isReadyForCollisionChecks = isReady;
    }

    public void initData() {
        updateVertices();
        updateCollilders();
        updateConstraints();
    }

    private void updateVertices() {
        Vector3[] vertices = singleton.tetMesh.getGlobalVertices();
        Debug.Log("DllInterface.updateVertices: 1st vert: " + vertices[0]);
        Debug.Log("DllInterface.updateVertices: 10st vert: " + vertices[10]);
        Debug.Log("DllInterface.updateVertices: 100t vert: " + vertices[100]);
        dll_setVertices(vertices, vertices.Length);
    }

    private void updateCollilders() {
        Vector3[] collPos, collSizes;
        ColliderType[] collTypes;
        ColliderManager.getColliderData(out collPos, out collSizes, out collTypes);
        dll_setColliders(collPos, collSizes, collTypes, collPos.Length);
    }

    private void updateConstraints() {
        //TODO: implement
    }
    
    public void testDataSetting() {
        int vertCount, collCount, constCount;
        dll_setTest(out vertCount, out collCount, out constCount);
        Debug.Log("set Test -- vert count: " + vertCount + "coll count: " + collCount + "constraint count: " + constCount);
    }

    public void getCollisionResult() {
        Debug.Log("There are "+dll_getVertexCountInColliders()+" vertices colliding with colliders");
    }


    public Vector3[] getVerticesFromDll() {
        Vector3[] resultArray = new Vector3[singleton.tetMesh.getVertices().Length];
        singleton.arrHandle = GCHandle.Alloc(resultArray, GCHandleType.Pinned);
        singleton.arrPtr = singleton.arrHandle.AddrOfPinnedObject();
        dll_getVertices(singleton.arrPtr);

        //Debug.Log("is dll vertex array correct? " + Enumerable.SequenceEqual(resultArray, singleton.tetMesh.getGlobalVertices()));
        return resultArray;
    }

    public void getCollidersFromDll() {
        int collCount = ColliderManager.getColliderList().Count;
        MyCollider[] dllColliders = new MyCollider[collCount];
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

        Vector3[] poss, sizes;
        ColliderType[] types;
        ColliderManager.getColliderData(out poss, out sizes, out types);

        Debug.Log("collider positions: " + Enumerable.SequenceEqual(poss, collPositions));
        Debug.Log("collider sizes: " + Enumerable.SequenceEqual(sizes, collSizes));
        Debug.Log("collider types: " + Enumerable.SequenceEqual(types, collTypes));
    }
}