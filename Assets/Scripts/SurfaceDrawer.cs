using System;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceDrawer :MonoBehaviour {
    private List<Vector3> vertices;
    private List<int> tris;
    private bool infoIsSet;
    [SerializeField] private Material lineMaterial;
    [SerializeField] private Color lineColor;
    [SerializeField] private float lineLength;

    private void Start() {
        vertices = new List<Vector3>();
        tris = new List<int>();
        infoIsSet = false;
    }

    public void setInfo(List<Vector3> verts, List<int> tris) {
        vertices = verts;
        this.tris = tris;
        infoIsSet = true;
        Debug.Log("SurfaceDrawer :: "+tris.Count/3);
    }

    private void OnDrawGizmos() {
        RenderEdges();
    }

    private void RenderEdges() {
        if (infoIsSet) {
            // set up
            Matrix4x4 mat = new Matrix4x4();
            mat.SetTRS(gameObject.transform.position, gameObject.transform.rotation, gameObject.transform.localScale);
            GL.PushMatrix();
            GL.MultMatrix(mat);
            GL.Begin(GL.LINES);
            lineMaterial.SetPass(0);
            // draw triangle edges
            Debug.Log(tris.Count);
            for (int triID = 0; triID < 1962 ; triID++) {
                // add verts 0/1/2/0
                GL.Vertex(vertices[triID * 3]);
                GL.Vertex(vertices[triID * 3 + 1]);
                GL.Vertex(vertices[triID * 3 + 1]);
                GL.Vertex(vertices[triID * 3 + 2]);
                GL.Vertex(vertices[triID * 3 + 2]);
                GL.Vertex(vertices[triID * 3]);
            }
            // set down
            GL.End();
            GL.PopMatrix();
        }
    }
}
