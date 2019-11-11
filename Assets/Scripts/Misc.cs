using System;
using UnityEngine;

public class Misc : MonoBehaviour{
    public static void logArray<T>(T[] array, string pretext, int start, int end) {
        string result = pretext + "";
        result += "len: " + array.Length + "__";
        for (int i = start; i <= end; i++) {
            result += array[i].ToString() + ", ";
        }
        Debug.Log(result);
    }

    public static void logArray<T>(T[] array, string pretext, int count) {
        string result = pretext + "";
        result += "len: " + array.Length + "__";
        for (int i = 0; i < Math.Min(array.Length, count); i++) {
            result += array[i].ToString() + ", ";
        }
        Debug.Log(result);
    }

    public static void logArray<T>(T[] array, string pretext) {
        logArray<T>(array, pretext, 10);
    }

    public static void logArray<T>(T[] array) {
        logArray<T>(array, "", 10);
    }
}
