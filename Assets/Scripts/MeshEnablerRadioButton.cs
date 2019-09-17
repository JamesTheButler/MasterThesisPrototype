using UnityEngine;

public class MeshEnablerRadioButton :MonoBehaviour {
    [SerializeField] private MeshEnabler meshEnabler;
    [SerializeField] private MeshType meshType;

    public void onValueChanged() {
        //meshEnabler.show(meshType);
    }
}

