using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

class MeshImporter {
    enum LineType {
        NONE,
        VERTICES,
        TETRAHEDRA
    }

    public static void import(string path, out List<Vector3> vertices, out List<int> tetrahedra) {
        string[] allLines = File.ReadAllLines(path);
        parse(allLines, out vertices, out tetrahedra);
    }

    private static void parse(string[] meshFileLines, out List<Vector3> vertices, out List<int> tetrahedra) {
        vertices = new List<Vector3>();
        tetrahedra = new List<int>();
        LineType lineType = LineType.NONE;
        int vertexCount = 0, tetrahedraCount = 0;

        foreach (string line in meshFileLines) {
            if (line.StartsWith("error"))
                Debug.LogError("Mesh File could not be read!");

            switch (lineType) {
                // find vertex/tetra specifier
                case LineType.NONE:
                    if (line.StartsWith("Vertices")) {
                        lineType = LineType.VERTICES;
                    } else if (line.StartsWith("Tetrahedra")) {
                        lineType = LineType.TETRAHEDRA;
                    }
                    break;

                // vertex mode
                case LineType.VERTICES:
                    // check if vertex mode ends
                    if (line.StartsWith("Triangles")) {
                        lineType = LineType.NONE;
                    }
                    // parse vertex count
                    else if (Regex.IsMatch(line, @"^\d*$")) {
                        if (!Int32.TryParse(line, out vertexCount)) {
                            vertexCount = -1;
                            Debug.LogError("vertex count could not be parsed");
                        }
                        //Debug.Log(vertexCount + " vertecies found.");
                    }
                    // parse vertex info into vec3
                    else if (Regex.IsMatch(line, @"^(-?\d*(.\d*(e-\d*)?)? ){3}0$")) {
                        float[] vertexInfo = { 0, 0, 0 };
                        string[] vertexInfoStrings = line.Split();
                        for (int i = 0; i <= 2; i++) {
                            vertexInfo[i] = float.Parse(vertexInfoStrings[i]);
                        }
                        vertices.Add(new Vector3(vertexInfo[0], vertexInfo[1], vertexInfo[2]));
                    }
                    // parse fail
                    else {
                        Debug.LogError("vertex info line could not be parsed: " + line);
                    }
                    break;

                // tetrahedra mode
                case LineType.TETRAHEDRA:
                    //detect end of file
                    if (line.StartsWith("End")) {
                        lineType = LineType.NONE;
                    }
                    //find tetrahedra count
                    else if (Regex.IsMatch(line, @"^\d*$")) {
                        if (!Int32.TryParse(line, out tetrahedraCount)) {
                            tetrahedraCount = -1;
                            Debug.LogError("Tetrahedra count could not be parsed");
                        }
                        //Debug.Log(tetrahedraCount + " tetrahedra found");
                    }
                    // parse tetrahedron info into 4 ints (indeces of vertecies)
                    else if (Regex.IsMatch(line, @"^(-?\d* ){4}0$")) {
                        int[] vertexIndeces = { 0, 0, 0, 0 };
                        string[] vertexIndexStrings = line.Split();
                        for (int i = 0; i <= 3; i++) {
                            vertexIndeces[i] = Int32.Parse(vertexIndexStrings[i]) - 1;
                            tetrahedra.Add(vertexIndeces[i]);
                        }
                    }
                    // parse fail
                    else {
                        Debug.LogError("Tetrahedra info could not be parsed: " + line);
                    }
                    break;
            }
        }
        // check for correctnes
        if (vertexCount != vertices.Count)
            Debug.LogError("Vertex count does not match count from file!" + vertexCount + " " + vertices.Count);
        if (tetrahedraCount != tetrahedra.Count / 4)
            Debug.LogError("Tetrahedra count does not match count from file!" + tetrahedraCount + " " + tetrahedra.Count / 4);
    }
}