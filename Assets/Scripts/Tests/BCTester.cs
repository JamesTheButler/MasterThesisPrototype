using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;



public class BCTester : MonoBehaviour, MovementListener { 
    //setup
    [DllImport("TestDll")]
    private static extern bool dll_bc_init();
    [DllImport("TestDll")]
    private static extern bool dll_bc_teardown();
    // setters
    [DllImport("TestDll")]
    private static extern void dll_bc_setVerts(Vector3[] verts, int count);
    [DllImport("TestDll")]
    private static extern void dll_bc_setTets(int[] tetrahedra, int count);
    [DllImport("TestDll")]
    private static extern void dll_bc_setFollowers(Vector3[] followers, int count);
    [DllImport("TestDll")]
    private static extern void dll_bc_updateVert(int id, float x, float y, float z);
    //getters
    [DllImport("TestDll")]
    private static extern void dll_bc_get_all(IntPtr verts, IntPtr tets, IntPtr fverts, IntPtr map, IntPtr bccs, IntPtr bcts);
    [DllImport("TestDll")]
    private static extern int dll_bc_getFollowerCount();
    [DllImport("TestDll")]
    private static extern void dll_bc_getFollowers(IntPtr tets);
    //[DllImport("TestDll")]
    //private static extern int dll_bc_getClosestTetCount();
    //[DllImport("TestDll")]
    //private static extern void dll_bc_getClosestTets(IntPtr tets);

    

    public class Vector4i {
        public int x, y, z, w;

        public Vector4i() {
            this.x = 0;
            this.y = 0;
            this.z = 0;
            this.w = 0;
        }

        public Vector4i(int x, int y, int z, int w) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
    }

    List<GameObject> controllVerts;
    List<Vector4i> tets;
    List<GameObject> followers;

    void Start () {
        int controllerId = 0;
        int followerId = 0;
        //grab data from scene
        controllVerts = new List<GameObject>();
        followers = new List<GameObject>();
        tets = new List<Vector4i>();
        for(int i=0; i<transform.childCount; i++) {
            Transform child = transform.GetChild(i);
            if (child.tag == "Controller") {
                controllVerts.Add(child.gameObject);
                child.gameObject.GetComponent<ControlSphere>().setId(controllerId);
                controllerId++;
                child.gameObject.GetComponent<ControlSphere>().setMovementListener(this);
            } else if (child.tag == "Follower") {
                followers.Add(child.gameObject);
                child.gameObject.GetComponent<Follower>().id = followerId;
                followerId++;
            }
        }
        tets.Add(new Vector4i(0,1,2,3));
        tets.Add(new Vector4i(1,2,3,4));
        //init dll
        Debug.Log(getControlVerts().Length);
        Debug.Log(tets.Count);
        Debug.Log(getFollowers().Length);
        dll_bc_setVerts(getControlVerts(), getControlVerts().Length);
        dll_bc_setTets(getTets(), tets.Count);
        dll_bc_setFollowers(getFollowers(), getFollowers().Length);

        dll_bc_init();
        //int[] cts = getClosestTetsFromDll();
        //Debug.Log(cts.Length);
        //for (int i = 0; i < cts.Length; i++) {
        //    Debug.Log(i+": "+cts[i]);
        //}
        debugTestDll();
    }

    private Vector3[] getControlVerts() {
        List<Vector3> result = new List<Vector3>();
        foreach(GameObject go in controllVerts)
            result.Add(go.transform.position);
        return result.ToArray();
    }

    private int[] getTets() {
        List<int> result = new List<int>();
        foreach(Vector4i tet in tets) {
            result.Add(tet.x);
            result.Add(tet.y);
            result.Add(tet.z);
            result.Add(tet.w);
        }
        return result.ToArray();
    }

    private Vector3[] getFollowers() {
        List<Vector3> result = new List<Vector3>();
        foreach (GameObject go in followers)
            result.Add(go.transform.position);
        return result.ToArray();
    }

    private void updateFollowers(Vector3[] newPos) {
        for (int i = 0; i < followers.Count; i++) {
            followers[i].transform.localPosition = newPos[i];
        }
    }

    //private int[] getClosestTetsFromDll() {
        //int[] resultArray = new int[dll_bc_getClosestTetCount()];
        //GCHandle arrHandle = GCHandle.Alloc(resultArray, GCHandleType.Pinned);
        //IntPtr arrPtr = arrHandle.AddrOfPinnedObject();
        //dll_bc_getClosestTets(arrPtr);
        //return resultArray;
    //}

    public Vector3[] getFollowersFromDll() {
        Vector3[] resultArray = new Vector3[dll_bc_getFollowerCount()];
        GCHandle arrHandle = GCHandle.Alloc(resultArray, GCHandleType.Pinned);
        IntPtr arrPtr = arrHandle.AddrOfPinnedObject();
        dll_bc_getFollowers(arrPtr);
        return resultArray;
    }

    private void OnDestroy() {
        dll_bc_teardown();
    }

    public void onPosChanged(int id, Vector3 newPos) {
        //Debug.Log("sphere nr " + id + " got moved to " + newPos);

        dll_bc_updateVert(id, newPos.x, newPos.y, newPos.z);
        updateFollowers(getFollowersFromDll());
    }

    private void debugTestDll() {
        Vector3[] vertArray = new Vector3[controllVerts.Count];
        GCHandle cv_arrHandle = GCHandle.Alloc(vertArray, GCHandleType.Pinned);
        IntPtr cv_arrPtr = cv_arrHandle.AddrOfPinnedObject();

        Vector4i[] tetArray = new Vector4i[tets.Count];
        GCHandle ct_arrHandle= GCHandle.Alloc(tetArray, GCHandleType.Pinned);
        IntPtr ct_arrPtr = ct_arrHandle.AddrOfPinnedObject();

        Vector3[] fVertArray = new Vector3[followers.Count];
        GCHandle fv_arrHandle = GCHandle.Alloc(fVertArray, GCHandleType.Pinned);
        IntPtr fv_arrPtr = fv_arrHandle.AddrOfPinnedObject();

        int[] map = new int[controllVerts.Count];
        GCHandle map_arrHandle = GCHandle.Alloc(map, GCHandleType.Pinned);
        IntPtr map_arrPtr = map_arrHandle.AddrOfPinnedObject();

        Vector4[] bccs = new Vector4[controllVerts.Count];
        GCHandle bcc_arrHandle = GCHandle.Alloc(bccs, GCHandleType.Pinned);
        IntPtr bcc_arrPtr = bcc_arrHandle.AddrOfPinnedObject();

        int[] bcts = new int[controllVerts.Count];
        GCHandle bct_arrHandle = GCHandle.Alloc(bcts, GCHandleType.Pinned);
        IntPtr bct_arrPtr = bct_arrHandle.AddrOfPinnedObject();

        dll_bc_get_all(cv_arrPtr, ct_arrPtr, fv_arrPtr, map_arrPtr, bcc_arrPtr, bct_arrPtr);
        Debug.Log(vertArray.Length + " " +tetArray.Length +" "+ fVertArray.Length + " " + map.Length + " " + bccs.Length + " " + bcts.Length);
        for(int i=0; i < bccs.Length; i++) {
            Debug.Log(i + ": " + bccs[i]);
        }
    }


    public void drawTet(Vector4i tet) {
        Vector3[] verts = getControlVerts();
        GL.Vertex3(verts[tet.x].x, verts[tet.x].y, verts[tet.x].z);
        GL.Vertex3(verts[tet.y].x, verts[tet.y].y, verts[tet.y].z);
        GL.Vertex3(verts[tet.x].x, verts[tet.x].y, verts[tet.x].z);
        GL.Vertex3(verts[tet.z].x, verts[tet.z].y, verts[tet.z].z);
        GL.Vertex3(verts[tet.x].x, verts[tet.x].y, verts[tet.x].z);
        GL.Vertex3(verts[tet.w].x, verts[tet.w].y, verts[tet.w].z);

        GL.Vertex3(verts[tet.y].x, verts[tet.y].y, verts[tet.y].z);
        GL.Vertex3(verts[tet.z].x, verts[tet.z].y, verts[tet.z].z);
        GL.Vertex3(verts[tet.y].x, verts[tet.y].y, verts[tet.y].z);
        GL.Vertex3(verts[tet.w].x, verts[tet.w].y, verts[tet.w].z);

        GL.Vertex3(verts[tet.z].x, verts[tet.z].y, verts[tet.z].z);
        GL.Vertex3(verts[tet.w].x, verts[tet.w].y, verts[tet.w].z);

    }

    static Material lineMaterial;
    static void CreateLineMaterial() {
        if (!lineMaterial) {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 1);
        }
    }

    public void OnRenderObject() {
        CreateLineMaterial();
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);
        GL.Begin(GL.LINES); 
        GL.Color(Color.red);
        for (int i = 0; i < tets.Count; ++i) {
            drawTet(tets[i]);
        }
        GL.End();
        GL.PopMatrix();
    }
}
