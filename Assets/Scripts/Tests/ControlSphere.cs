using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface MovementListener {
    void onPosChanged(int id, Vector3 newPos);
}

public class ControlSphere : MonoBehaviour {
    private int id;
    List<MovementListener> listeners;
    Vector3 oldPos;

    private Vector3 mOffset;
    private float mZCoord;

    private void Start() {
        listeners = new List<MovementListener>();
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

    public void addMovementListener(MovementListener l) {
       // listeners.Add(l);
    }

    private void OnMouseDrag() {
        Vector3 pos = GetMouseAsWorldPoint() + mOffset; ;
        transform.position = pos;
        if (pos != oldPos && listeners.Count!=0) {
    
            foreach(MovementListener l in listeners) {
                l.onPosChanged(this.id, pos);
            }
        }
    }
}
