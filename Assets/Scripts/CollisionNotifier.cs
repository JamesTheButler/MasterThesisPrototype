using System.Collections.Generic;
using UnityEngine;

public class CollisionNotifier : MonoBehaviour {
    [SerializeField]private GameObject attachedObject;

    private List<ICollisionEventHandler> listeners;

    private void Awake() {
        listeners = new List<ICollisionEventHandler>(attachedObject.GetComponents<ICollisionEventHandler>());
    }

    private void OnTriggerStay(Collider collider) {
        foreach (ICollisionEventHandler handler in listeners)
            if (handler != null)
                handler.onTriggerStay(collider);
    }
}