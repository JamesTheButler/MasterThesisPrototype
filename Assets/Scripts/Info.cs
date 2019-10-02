using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Info : MonoBehaviour {
    public GameObject particle;

    public void getResult(Vector3 vec, Vector3 rot) {
        transform.Rotate(rot);
        particle.transform.localPosition = vec;
        Debug.Log("actual rotated vertex" + particle.transform.position);
    }
}
