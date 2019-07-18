﻿using System;
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

    public Vector3[] getVertices() { return vertices; }
    public List<Constraint> getConstraints() { return constraints; }

    public void setTetMeshData(List<Vector3> vertices, List<int> tetrahedra, List<Vector3> surfaceVertices, List<int> surfaceTriangles) {
        //tet mesh info
        this.vertices = vertices.ToArray();
        this.tetrahedra = tetrahedra.ToArray();
        tetCount = tetrahedra.Count / 4;
        //surface info
        this.surfaceVertices = surfaceVertices.ToArray();
        this.surfaceTriangles = surfaceTriangles.ToArray();
        generateSurfaceMeshObject();
        //gameObject.GetComponent<SurfaceDrawer>().setInfo(surfaceVertices, surfaceTriangles);
        //index surface indices
        surfaceVertexIndeces = indexSubsetVertices(surfaceVertices, vertices).ToArray();
        //constraints
        constraints = generateDistanceConstraints();
    }

    private void generateSurfaceMeshObject() {
        Mesh mesh = new Mesh();
        MeshFilter filter = tetMeshGameObject.AddComponent<MeshFilter>();
        filter.mesh = mesh;
        MeshRenderer renderer = tetMeshGameObject.AddComponent<MeshRenderer>();
        renderer.material = tetMeshMaterial;
        mesh.vertices = surfaceVertices;
        mesh.triangles = surfaceTriangles;
    }

    // Finds all vertices from a set in its superset and saves their indices
    private List<int> indexSubsetVertices(List<Vector3> subSet, List<Vector3> superSet) {
        List<int> indeces = new List<int>();
        //find all surface vertices in all vertices
        foreach (Vector3 vertex in subSet) {
            if (superSet.Contains(vertex)) {
                indeces.Add(superSet.IndexOf(vertex));
            }
        }
        Debug.Log(indeces.Count + "/" + subSet.Count + " surface vertices indexed.");
        return indeces;
    }

   /*private void generateConstraints(int[] vertices, List<Vector3> allVertices, ConstraintType type) {
        switch (type) {
            case ConstraintType.DISTANCE:
                generateDistanceConstraints();
                break;
        }
    }*/

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