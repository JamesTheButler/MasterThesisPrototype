using UnityEngine;

public enum ColliderType {
    NONE = 0,
    BOX = 1,
    SPHERE = 2,
    CAPSULE = 3,
}

public struct ColliderData {
    public Vector3 position;
    public Vector3 size;
    public ColliderType type;
    public int id;

    public override string ToString() {
        return "pos: " + position + "size: " + size + "type: " + type + "id: " + id;
    }
}
