using UnityEngine;
/// <summary>
/// Used to interface with the trigger event invoker
/// </summary>
public interface ICollisionEventHandler {
    void onTriggerStay(Collider otherCollider);
}