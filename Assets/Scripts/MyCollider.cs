using UnityEngine;

public enum ColliderType {
    NONE = 0,
    BOX = 1
}

public struct MyColliderData {
    public Vector3 position;
    public Vector3 size;
    public ColliderType type;
    public int id;

    public override string ToString() {
        return "pos: " + position+ "size: " + size + "type: " + type + "id: " + id;
    }
}

public class MyCollider : MonoBehaviour {
    private MyColliderData collData;
    public MyColliderData getColliderData() { return collData; }
    [SerializeField]private ColliderType type;

    public void setId(int id) { collData.id = id; }
    public void setType(ColliderType type) { this.type = type; }

    public int getId() { return collData.id; }
    public Vector3 getPosition() { return collData.position; }
    public Vector3 getSize() { return collData.size; }
    public ColliderType getType() { return collData.type; }

    private void Awake() {
        collData.position = gameObject.transform.localPosition;
        collData.size = gameObject.transform.localScale;
        collData.type = type;
    }
}