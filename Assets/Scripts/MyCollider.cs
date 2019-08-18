using System;
using UnityEngine;

public enum ColliderType {
    NONE = 0,
    BOX = 1
}

public struct MyColliderData {
    public Vector3 position;
    public Vector3 size;
    public ColliderType type;
}

public class MyCollider : MonoBehaviour {
    private MyColliderData collData;
    public MyColliderData getColliderData() { return collData; }
    [SerializeField]private ColliderType type;

    public Vector3 getPosition() { return collData.position; }
    public Vector3 getSize() { return collData.size; }
    /// <summary> Returns the colliders type (i.e. box, sphere, etc). </summary>
    public ColliderType getType() { return collData.type; }

    private void Start() {
        setInfo();
        registerSelf();
    }
    
    private void setInfo() {
        collData.position = gameObject.transform.localPosition;
        collData.size = gameObject.transform.localScale;
        collData.type = type;
    }

    public void setType(ColliderType type) {
        this.type = type;
    }

    private void registerSelf() {
        ColliderManager.registerCollider(this);
    }
}