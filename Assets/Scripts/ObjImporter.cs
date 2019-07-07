using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ObjImporter{
    public static void import(string path, out List<Vector3> verts, out List<int> tris) {
        string[] allLines = File.ReadAllLines(path);
        parse(allLines, out verts, out tris);
    }

    private static void parse(string[] allLines, out List<Vector3> verts, out List<int> tris) {
        verts = new List<Vector3>();
        tris = new List<int>();

        float[] vertexInfo = { 0, 0, 0 };
        string[] infoString = { };
        Debug.Log("line count " + allLines.Length);
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
        Debug.Log("vert count " + verts.Count);
        Debug.Log("tri count " + tris.Count);
    }
}