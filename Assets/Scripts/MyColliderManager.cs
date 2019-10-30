using System.Collections.Generic;
using UnityEngine;

public class MyColliderManager : MonoBehaviour {
    public GameObject colliderFolder;

    private static MyColliderManager singleton;
    private List<MyCollider> colliders;

    private void Awake() {
        singleton = this;
    }

    private void Start() {
        colliders = new List<MyCollider>(colliderFolder.GetComponentsInChildren<MyCollider>());
        for (int i = 0; i < colliders.Count; i++)
            colliders[i].setId(i);
        DllInterface.getSingleton().setupColliders();
    }

    public static int registerMyCollider(MyCollider coll) {
        singleton.colliders.Add(coll);
        return singleton.colliders.Count-1;
    }

    public static List<MyCollider> getColliderList() {
        return singleton.colliders;
    }

    public static void getColliderData(out Vector3[] colliderPositions, out Vector3[] colliderSizes, out ColliderType[] colliderTypes) {
        int collCount = singleton.colliders.Count;
        colliderPositions = new Vector3[collCount];
        colliderSizes = new Vector3[collCount];
        colliderTypes = new ColliderType[collCount];

        for (int i=0; i< collCount; i++) {
            colliderPositions[i] = singleton.colliders[i].getPosition();
            colliderSizes[i] = singleton.colliders[i].getSize();
            colliderTypes[i] = singleton.colliders[i].getType();
            Debug.Log("coll nr #" + i + "coll pos:" + colliderPositions[i] + "coll size:" + colliderSizes[i] + "coll type:" + colliderTypes[i]);
        }
    }
}