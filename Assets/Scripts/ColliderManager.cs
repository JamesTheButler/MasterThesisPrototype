using System;
using System.Collections.Generic;
using UnityEngine;

public class ColliderManager : MonoBehaviour {
    private static ColliderManager singleton;
    private List<MyCollider> colliders;

    private void Awake() {
        singleton = this;
        colliders = new List<MyCollider>();
    }

    public static void registerCollider(MyCollider coll) {
        singleton.colliders.Add(coll);
    }

    public static List<MyCollider> getColliderList() {
        return singleton.colliders;
    }

    public static void getColliderData(out Vector3[] colliderPositions, out Vector3[] colliderSizes) {
        int collCount = singleton.colliders.Count;
        colliderPositions = new Vector3[collCount];
        colliderSizes = new Vector3[collCount];
        
        for(int i=0; i< collCount; i++) {
            colliderPositions[i] = singleton.colliders[i].getPosition();
            colliderSizes[i] = singleton.colliders[i].getSize();
        }
    }
}
