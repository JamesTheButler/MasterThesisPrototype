using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DllInterface : MonoBehaviour {
    [SerializeField] private TetrahedralMesh tetMesh;
    public static DllInterface singleton;

    [DllImport("PlasticDeformationDll")]
    private static extern void setVertices(Vector3[] verts, int vertexCount);
    //private static extern void setVertices(Vector3[] verts, int vertexCount, out float result, out int vertCount);
    [DllImport("PlasticDeformationDll")]
    private static extern void getVertexCountInColliders(Vector3[] verts, int elementCount, Vector3[] colliderPositions, Vector3[] colliderSizes, int colliderCount, out int collisionCount);
    [DllImport("PlasticDeformationDll")]
    private static extern void setTestArray(int[] inputArray, int elementCount);
    [DllImport("PlasticDeformationDll")]
    private static extern void dataTest(out int result);
    [DllImport("PlasticDeformationDll")]
    private static extern void setTest(int input);
    [DllImport("PlasticDeformationDll")]
    private static extern void getTest(out int result);
    [DllImport("PlasticDeformationDll")]
    private static extern int returnTest();


    private void Start() {
        singleton = this;
    }

    void Update () {
        //physicsCalculations();
	}

    public static void initVertices(Vector3[] vertices) {
        //TODO: implement
    }

    public static void initCollilders(MyCollider[] constraints) {
        //TODO: implement
    }

    public static void initConstraints(Constraint[] constraints) {
        //TODO: implement
    }

    public static void physicsCalculations() {
        //gather data and pre-process data
        // - vertices
        singleton.tetMesh.getVertices();
        // - constraints
        singleton.tetMesh.getConstraints();
        // - colliders
        ColliderManager.getColliderList();
        //send data to dll
        //float result=0f;
        //int vertCount=0;
        //setVertices(singleton.tetMesh.getVertices(), singleton.tetMesh.getVertices().Length * 3, out result, out vertCount);
        Vector3[] colliderPositions, colliderSizes;
        ColliderManager.getColliderData(out colliderPositions, out colliderSizes);
        int collisionCount = 0;
        getVertexCountInColliders(singleton.tetMesh.getVertices(), singleton.tetMesh.getVertices().Length * 3, colliderPositions, colliderSizes, colliderPositions.Length, out collisionCount);
        //process results
        //Debug.Log("setVertices: "+result + " "+ vertCount);
        Debug.Log("getVertexCountInColliders: " + collisionCount);
    }
}
