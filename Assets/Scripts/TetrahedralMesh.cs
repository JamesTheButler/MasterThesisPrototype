using System;
using System.Collections.Generic;
using UnityEngine;

public class TetrahedralMesh : MonoBehaviour {
    private Vector3[] vertices;
    private int[] tetrahedra;
    private int tetCount; 

    private Vector3[] surfaceVertices;
    private int[] surfaceTriangles;
    private int surfaceTriangleCount;

    private int[] surfaceVertexIndeces;

    private List<Constraint> constraints;

    [SerializeField] private GameObject tetMeshGameObject;
    [SerializeField] private Material tetMeshMaterial;
    [SerializeField] private GameObject surfaceMeshGO;

    public Vector3[] getVertices() { return vertices; }
    public List<Constraint> getConstraints() { return constraints; }

    /// <summary>
    /// Returns vertices with global transform.
    /// </summary>
    public Vector3[] getGlobalVertices() {
        Vector3[] globalVerts = (Vector3[])vertices.Clone();
        Vector3 position = tetMeshGameObject.transform.position;
        Quaternion rotation = tetMeshGameObject.transform.rotation;

        for(int i = 0; i < globalVerts.Length; i++) {
            globalVerts[i] = rotation * globalVerts[i];
        }
        for (int i = 0; i < globalVerts.Length; i++) {
            globalVerts[i] += position;
        }
        return globalVerts;
    }

    /// <summary>
    /// Sets all data to tet mesh.
    /// </summary>
    public void setTetMeshData(List<Vector3> vertices, List<int> tetrahedra, List<Vector3> surfaceVertices, List<int> surfaceTriangles) {
        //tet mesh info
        this.vertices = vertices.ToArray();
        this.tetrahedra = tetrahedra.ToArray();
        tetCount = tetrahedra.Count / 4;
        //surface info
        this.surfaceVertices = surfaceVertices.ToArray();
        this.surfaceTriangles = surfaceTriangles.ToArray();
        generateSurfaceMeshObject();
        //index surface indices
        surfaceVertexIndeces = indexSubsetVertices(surfaceVertices, vertices).ToArray();
        //constraints
        constraints = generateDistanceConstraints();
    }

    /// <summary>
    /// Returns the translation and rotation of the tet mesh using the parameters as output.
    /// </summary>
    public void getTransforms(out Vector3 translation, out Quaternion rotation) {
        translation = tetMeshGameObject.transform.position;
        rotation = tetMeshGameObject.transform.rotation;
    }

    /// <summary>
    /// Generates unity mesh with the surface vertices of the tet mesh.
    /// </summary>
    private void generateSurfaceMeshObject() {
        Mesh mesh = new Mesh();
        MeshFilter filter = surfaceMeshGO.AddComponent<MeshFilter>();
        filter.mesh = mesh;
        MeshRenderer renderer = surfaceMeshGO.GetComponent<MeshRenderer>();
        renderer.material = tetMeshMaterial;
        mesh.vertices = surfaceVertices;
        mesh.triangles = surfaceTriangles;
    }

    /// <summary>
    ///  Finds all vertices from a set in its superset and saves their indices.
    /// </summary>
    private List<int> indexSubsetVertices(List<Vector3> subSet, List<Vector3> superSet) {
        List<int> indeces = new List<int>();
        //find all surface vertices in all vertices
        foreach (Vector3 vertex in subSet) {
            if (superSet.Contains(vertex)) {
                indeces.Add(superSet.IndexOf(vertex));
            }
        }
        Debug.Log("[INFO] "+indeces.Count + "/" + subSet.Count + " surface vertices indexed.");
        return indeces;
    }

   /*private void generateConstraints(int[] vertices, List<Vector3> allVertices, ConstraintType type) {
        switch (type) {
            case ConstraintType.DISTANCE:
                generateDistanceConstraints();
                break;
        }
    }*/

    /// <summary>
    /// Generates distance constrains. Creates 6 constraints per tetrahedron and sets rest distance to current distance.
    /// </summary>
    private List<Constraint> generateDistanceConstraints() {
        Vector3[] tetVertices = new Vector3[4];
        int[] tetVertexIDs = new int[4];
        List<Constraint> distanceConstraints = new List<Constraint>();
        //for each tetrahdedron
        for (int tetId=0; tetId<tetCount; tetId++) {
            //get 4 vertices + their ids
            for(int i = 0; i<4; i++) {
                int vertexID = tetrahedra[tetId * 4 + i];
                tetVertices[i] = vertices[vertexID];
                tetVertexIDs[i] = vertexID;
            }
            distanceConstraints.Add(new Constraint(new int[] { tetVertexIDs[0], tetVertexIDs[1] }, Vector3.Distance(tetVertices[0], tetVertices[1]), ConstraintType.DISTANCE));
            distanceConstraints.Add(new Constraint(new int[] { tetVertexIDs[0], tetVertexIDs[2] }, Vector3.Distance(tetVertices[0], tetVertices[2]), ConstraintType.DISTANCE));
            distanceConstraints.Add(new Constraint(new int[] { tetVertexIDs[0], tetVertexIDs[3] }, Vector3.Distance(tetVertices[0], tetVertices[3]), ConstraintType.DISTANCE));
            distanceConstraints.Add(new Constraint(new int[] { tetVertexIDs[1], tetVertexIDs[2] }, Vector3.Distance(tetVertices[1], tetVertices[2]), ConstraintType.DISTANCE));
            distanceConstraints.Add(new Constraint(new int[] { tetVertexIDs[1], tetVertexIDs[3] }, Vector3.Distance(tetVertices[1], tetVertices[3]), ConstraintType.DISTANCE));
            distanceConstraints.Add(new Constraint(new int[] { tetVertexIDs[2], tetVertexIDs[3] }, Vector3.Distance(tetVertices[2], tetVertices[3]), ConstraintType.DISTANCE));
        }
        return distanceConstraints;
    }
}