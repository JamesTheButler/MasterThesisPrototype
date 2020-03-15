using UnityEngine;

public interface MovementListener {
    void onPosChanged(int id, Vector3 newPos);
}

public class ControlSphere : MonoBehaviour {
    [SerializeField] private int id;
    MovementListener listener = null;
    Vector3 oldPos;

    private Vector3 mOffset;
    private float mZCoord;

    private void Awake() {
        oldPos = transform.position;
    }

    void OnMouseDown() {
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();
    }

    private Vector3 GetMouseAsWorldPoint() {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    public void setId(int id) {
        this.id = id;
    }

    public void setMovementListener(MovementListener l) {
        listener = l;
    }

    private void OnMouseDrag() {
        /*Vector3 pos = GetMouseAsWorldPoint() + mOffset; ;
        transform.position = pos;
        if (pos != oldPos && listener != null) {
            listener.onPosChanged(id, pos);
        }*/
    }

    private void OnMouseUp() {
        Vector3 pos = GetMouseAsWorldPoint() + mOffset; ;
        transform.position = pos;
        if (pos != oldPos && listener != null) {
            listener.onPosChanged(id, pos);
        }
    }
}
