using UnityEngine;

public class MyCollider : MonoBehaviour {
    private Vector3 position;
    private Vector3 size;

    public Vector3 getPosition() { return position; }
    public Vector3 getSize() { return size; }

    private void Start() {
        setInfo();
        registerSelf();
    }

    protected virtual void setInfo() {
        position = gameObject.transform.position;
        size = gameObject.transform.localScale;
    }

    private void registerSelf() {
        ColliderManager.registerCollider(this);
    }
}