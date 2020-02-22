using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstraintTester : MonoBehaviour {

    class ivec4 {
        public int x, y, z, w;

        public ivec4() {
            this.x = 0;
            this.y = 0;
            this.z = 0;
            this.w = 0;
        }

        public ivec4(int x, int y, int z, int w) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
    }

    List<Vector3> verts;
    List<ivec4> tets;
    List<bool> isMovable;

    public void init() {
        verts = new List<Vector3>();
        tets = new List<ivec4>();
        isMovable = new List<bool>();

        verts.Add(new Vector3(0, 2, 0));
        verts.Add(new Vector3(0, 0, 0));
        verts.Add(new Vector3(1, 0, 0));
        verts.Add(new Vector3(-1, 0.5f, 1));

        tets.Add(new ivec4(0,1,2,3));

        for(int i=0; i < verts.Count; i++) {
            isMovable.Add(true);
        }
        isMovable[0] = false;
    }

    private void Start() {
        init();
    }

    private void Update() {
        for(int i=0; i<verts.Count; i++) {
            if (isMovable[i]) {
                verts[i] += (new Vector3(0, -5, 0)*Time.deltaTime);
            }
        }
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
        ivec4 tet = tets[tetId];
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
}
