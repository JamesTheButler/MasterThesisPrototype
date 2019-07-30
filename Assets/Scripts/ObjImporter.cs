using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ObjImporter{
    public static void import(string path, out List<Vector3> verts, out List<int> tris, out List<Vector3> normals) {
        string[] allLines = File.ReadAllLines(path);
        parse(allLines, out verts, out tris);
        calculateNormals(verts, tris, out normals);

        Debug.Log("[INFO] .obj import: " + verts.Count + " verts, " + tris.Count / 3 + " tris.");
    }

    public static void import(string path, out List<Vector3> verts, out List<int> tris) {
        string[] allLines = File.ReadAllLines(path);
        parse(allLines, out verts, out tris);
        Debug.Log("[INFO] .obj import: " + verts.Count + " verts, " + tris.Count/3 + " tris.");
    }

    private static void parse(string[] allLines, out List<Vector3> verts, out List<int> tris) {
        verts = new List<Vector3>();
        tris = new List<int>();


        float[] vertexInfo = { 0, 0, 0 };
        string[] infoString = { };
        //Debug.Log("line count " + allLines.Length);
        foreach (string line in allLines) {
            if (line.StartsWith("v")) {
                infoString = line.Substring(2).Split();
                for (int i = 0; i <= 2; i++) {
                    vertexInfo[i] = float.Parse(infoString[i]);
                }
                verts.Add(new Vector3(vertexInfo[0], vertexInfo[1], vertexInfo[2]));
            } else if (line.StartsWith("f")) {
                infoString = line.Substring(2).Split();
                for (int i = 0; i <= 2; i++) {
                    int info = Int32.Parse(infoString[i])-1;
                    tris.Add(info);
                }
            }
        }
        //Debug.Log("vert count " + verts.Count+ ", tri count " + tris.Count / 3);
    }

    private static void calculateNormals(List<Vector3> verts, List<int> tris, out List<Vector3> normals) {
        normals = new List<Vector3>();
        //initialize empty normals
        for(int i =0; i < verts.Count; i++) {
            normals.Add(new Vector3(0, 0, 0));
        }
        Debug.Log(tris.Count + " /3 = " + tris.Count / 3);
        //for (int tri = 0; tri < tris.Count / 3; tri++) {
        for (int tri = 0; tri < 3; tri++) {
            Vector3 vec1, vec2;
            vec1 = verts[tris[tri * 3 + 2]] - verts[tris[tri * 3]];
            vec2 = verts[tris[tri * 3 + 1]] - verts[tris[tri * 3]];

            Vector3 faceNormal = Vector3.Normalize(Vector3.Cross(vec1, vec2));

            for(int triVertex = 0; triVertex <=2; triVertex++) {
                int vertexId = tris[tri * 3 + triVertex];
               // Debug.Log(normals.Count + ", " + vertexId);
                normals[vertexId] = faceNormal;
            }
        }
    }
}