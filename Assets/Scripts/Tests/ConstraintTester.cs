using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;
using System;

public class ConstraintTester : MonoBehaviour, MovementListener {
    // solve
    [DllImport("TestDll")]
    private static extern void dll_solve();
    [DllImport("TestDll")]
    private static extern void dll_apply_gravity();
    [DllImport("TestDll")]
    private static extern void dll_solve_distance_j();
    [DllImport("TestDll")]
    private static extern void dll_solve_volume_j();
     [DllImport("TestDll")]
    private static extern void dll_solve_distance_gs();
    [DllImport("TestDll")]
    private static extern void dll_solve_volume_gs();
    [DllImport("TestDll")]
    private static extern void dll_solve_j();
    [DllImport("TestDll")]
    private static extern void dll_solve_gs();
    //setup
    [DllImport("TestDll")]
    private static extern bool dll_init();
    [DllImport("TestDll")]
    private static extern bool dll_teardown();
    // setters
    [DllImport("TestDll")]
    private static extern void dll_setVerts(Vector3[] verts, int count);
    [DllImport("TestDll")]
    private static extern void dll_setTets(int[] tetrahedra, int count);
    [DllImport("TestDll")]
    private static extern void dll_setIterationCount(int iterationCount);
    [DllImport("TestDll")]
    private static extern void dll_setFallSpeed(float speed);
    [DllImport("TestDll")]
    private static extern void dll_setIsMovable(int[] bools, int count);
    //getters
    [DllImport("TestDll")]
    private static extern void dll_getTets(IntPtr tets);
    [DllImport("TestDll")]
    private static extern int dll_getTetCount();
    [DllImport("TestDll")]
    private static extern void dll_getVerts(IntPtr verts);
    [DllImport("TestDll")]
    private static extern int dll_getVertCount();
    [DllImport("TestDll")]
    private static extern int dll_getIterationCount();
    [DllImport("TestDll")]
    private static extern void dll_updateVert(int id, float x, float y, float z);

    class Vector4i {
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

    List<GameObject> verts;
    List<Vector4i> tets;
    List<bool> isMovable;

    public GameObject sphereFolder;

    private void Start() {
        init();
        dll_setIterationCount(5);
        dll_setFallSpeed(2.0f);
        dll_setVerts(getControlVerts(), verts.Count);
        dll_setTets(toIntArray(tets), tets.Count);
        dll_setIsMovable(getIntBoolArray(isMovable), isMovable.Count);
        dll_init();
    }

    private void Update() {
        //dll_solve();
        //dll_apply_gravity();
        //dll_solve_distance_gs();
        dll_solve_volume_gs();
        updateVertices(getVertsFromDll());
    }

    private Vector3[] getControlVerts() {
        List<Vector3> result = new List<Vector3>();
        foreach (GameObject go in verts)
            result.Add(go.transform.position);
        return result.ToArray();
    }

    public void init() {
        int controllerId = 0;
        verts = new List<GameObject>();
        for (int i = 0; i < sphereFolder.transform.childCount; i++) {
            GameObject child = sphereFolder.transform.GetChild(i).gameObject;
            if (child.tag == "Controller") {
                verts.Add(child);
                child.GetComponent<ControlSphere>().setMovementListener(this);
                child.GetComponent<ControlSphere>().setId(controllerId);
                controllerId++;
            }
        }

        tets = new List<Vector4i>();
        tets.Add(new Vector4i(0, 1, 2, 4));
        tets.Add(new Vector4i(0, 2, 3, 4));
        tets.Add(new Vector4i(2, 3, 4, 5));

        isMovable = new List<bool>();
        for(int i=0; i < verts.Count; i++) {
            isMovable.Add(true);
        }
        isMovable[0] = false;
    }

    public void applyGravity() {
        dll_apply_gravity();
        updateVertices(getVertsFromDll());
    }
    public void solveDistance() {
        dll_solve_distance_j();
        updateVertices(getVertsFromDll());
    }
    public void solveVolume() {
        dll_solve_volume_j();
        updateVertices(getVertsFromDll());
    }
    public void solveBoth() {
        dll_solve_volume_j();
        dll_solve_distance_j();
        updateVertices(getVertsFromDll());
    }

    private int[] toIntArray(List<Vector4i> ivec4Array) {
        List<int> result = new List<int>();
        foreach (Vector4i i in ivec4Array) {
            result.Add(i.x);
            result.Add(i.y);
            result.Add(i.z);
            result.Add(i.w);
        }
        return result.ToArray();
    }

    public int[] getIntBoolArray(List<bool> bools) {
        List<int> ints = new List<int>();
        for(int i=0; i<bools.Count; i++) {
            ints.Add(bools[i] ? 1 : 0);
        }
        return ints.ToArray();
    }

    private void logDllVerts() {
        string s = "";
        Vector3[] verts = getVertsFromDll();
        for(int i=0; i<verts.Length; i++) {
            s += verts[i] + ", ";
        }
        Debug.Log(s);
    }

    private void updateVertices(Vector3[] newVerts) {
        for (int i=0; i<verts.Count; i++) {
            verts[i].transform.position = newVerts[i];
        }
    }

    public Vector3[] getVertsFromDll() {
        Vector3[] resultArray = new Vector3[dll_getVertCount()];
        GCHandle arrHandle = GCHandle.Alloc(resultArray, GCHandleType.Pinned);
        IntPtr arrPtr = arrHandle.AddrOfPinnedObject();
        dll_getVerts(arrPtr);
        return resultArray;
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
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    public void drawTet(int tetId) {
        Vector4i tet = tets[tetId];
        List<Vector3> verts = new List<Vector3>(getControlVerts());
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

    public void OnRenderObject() {
        CreateLineMaterial();
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);
        GL.Begin(GL.LINES);
        GL.Color(Color.red);
        for (int i = 0; i < tets.Count; ++i) {
            drawTet(i);
        }
        GL.End();
        GL.PopMatrix();
    }


    private void OnDestroy() {
        dll_teardown();
    }

    public void onPosChanged(int id, Vector3 newPos) {
        dll_updateVert(id, newPos.x, newPos.y, newPos.z);
        Debug.Log("onPosChanged " + id + ": "+newPos);
    }
}
