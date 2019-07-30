using System;
using UnityEngine;

public enum ColliderType {
    NONE = 0,
    BOX = 1
}

public class MyCollider : MonoBehaviour {
    private Vector3 position;
    private Vector3 size;
    [SerializeField]private ColliderType type;

    public Vector3 getPosition() { return position; }
    public Vector3 getSize() { return size; }
    public ColliderType getType() { return type; }

    private void Start() {
        setInfo();
        registerSelf();
    }

    protected virtual void setInfo() {
        position = gameObject.transform.position;
        size = gameObject.transform.localScale;
        type = ColliderType.BOX;
    }

    private void registerSelf() {
        ColliderManager.registerCollider(this);
    }

    internal int getColliderType() {
        throw new NotImplementedException();
    }
}