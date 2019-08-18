using System.Collections.Generic;
using UnityEngine;

public class DisableMeshRenderer : MonoBehaviour {
    [SerializeField] MeshRenderer mr;
    private static List<DisableMeshRenderer> disablers;

    private void Start() {
        if (disablers == null)
            disablers = new List<DisableMeshRenderer>();
        disablers.Add(this);
    }
    
    public void toggle(bool isOn) {
        Debug.Log("drm got toggled");
        hideAll();
        mr.enabled = isOn;
    }

    private void hideAll() {
        foreach(DisableMeshRenderer disabler in disablers) {
            disabler.toggle(false);
        }
    }
}
