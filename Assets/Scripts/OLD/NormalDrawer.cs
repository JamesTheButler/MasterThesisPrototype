using UnityEngine;
[System.Obsolete("We do not need normals any more.")]
class NormalDrawer :MonoBehaviour {
    public GameObject go;
    public Material material;
    public Color color = Color.blue;
    public float lineLength = 1f;


    private void OnDrawGizmos() {
        RenderNormals();
    }

    private void RenderNormals() {
        Vector3[] vertices = go.GetComponent<MeshFilter>().mesh.vertices;
        Vector3[] normals = go.GetComponent<MeshFilter>().mesh.normals;
        int vertCount = vertices.Length;

        Matrix4x4 mat = new Matrix4x4();
        mat.SetTRS(go.transform.position, go.transform.rotation, go.transform.localScale);
        GL.PushMatrix();
        GL.MultMatrix(mat);
        GL.Begin(GL.LINES);
        material.SetPass(0);

        GL.Color(color);
        for (int i = 0; i < vertCount; i++) {
            GL.Vertex(vertices[i]);
            GL.Vertex(vertices[i] + Vector3.Normalize(vertices[i] + normals[i]) * lineLength);
        }
        GL.End();
        GL.PopMatrix();
    }
}
