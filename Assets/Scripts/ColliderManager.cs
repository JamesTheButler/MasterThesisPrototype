using System.Collections.Generic;
using UnityEngine;

public class ColliderManager : MonoBehaviour {
    public GameObject colliderFolder;

    private static ColliderManager singleton;

    private List<Collider> colliders;

    private void Awake() {
        singleton = this;
    }

    private void Start() {
        colliders = new List<Collider>(colliderFolder.GetComponentsInChildren<Collider>());
        for(int i=0; i<colliders.Count; i++) {
            colliders[i].gameObject.GetComponent<Indexer>().Id = i;
        }
        DllInterface.getSingleton().setupColliders();
    }

    public static List<Collider> getColliderList() {
        return singleton.colliders;
    }

    public static int getColliderCount() {
        return singleton.colliders.Count;
    }

    public static void getColliderData(out Vector3[] colliderPositions, out Vector3[] colliderSizes, out ColliderType[] colliderTypes, out int collCount) {
        collCount = singleton.colliders.Count;
        colliderPositions = new Vector3[collCount];
        colliderSizes = new Vector3[collCount];
        colliderTypes = new ColliderType[collCount];

        for (int i=0; i< collCount; i++) {
            colliderPositions[i] = singleton.colliders[i].transform.position;
            colliderSizes[i] = singleton.colliders[i].transform.localScale;
            object type = singleton.colliders[i].GetType();
            if (type == typeof(BoxCollider)) {
                colliderTypes[i] = ColliderType.BOX;
            } else if (type == typeof(CapsuleCollider)) {
                colliderTypes[i] = ColliderType.CAPSULE;
            } else if (type == typeof(SphereCollider)) {
                colliderTypes[i] = ColliderType.SPHERE;
            } else {
                colliderTypes[i] = ColliderType.NONE;
            }
        }
    }
}