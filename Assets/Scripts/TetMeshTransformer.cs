using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Obsolete("Old")]
//TODO: remove
public class TetMeshTransformer : MonoBehaviour {
    [SerializeField] private GameObject tetMeshGO;
    private Vector3 basePosition, baseRotation, baseScale;
    [SerializeField] private GameObject positionGroup, rotationGroup, scaleGroup;
    public string namePrefix;
    bool isInitialized = false;

    private void Start() {
        writeDataToUI();
        basePosition = tetMeshGO.transform.localPosition;
        baseRotation = tetMeshGO.transform.localRotation.eulerAngles;
        baseScale = tetMeshGO.transform.localScale;
        isInitialized = true;
    }

    public void getDataFromUI() {
        if (!isInitialized)
            return;
        tetMeshGO.transform.localPosition = getDataFromUIGroup(positionGroup);
        tetMeshGO.transform.localRotation = Quaternion.Euler(getDataFromUIGroup(rotationGroup));
        tetMeshGO.transform.localScale = getDataFromUIGroup(scaleGroup);
    }

    private Vector3 getDataFromUIGroup(GameObject group) {
        Vector3 data = new Vector3();
        foreach(TMP_InputField input in group.GetComponentsInChildren<TMP_InputField>()) {
            if (input.name == namePrefix + "x")
                data += new Vector3(float.Parse(input.text), 0, 0);
            else if (input.name == namePrefix + "y")
                data += new Vector3(0,float.Parse(input.text), 0);
            else if (input.name == namePrefix + "z")
                data += new Vector3(0,0,float.Parse(input.text));
        }
        return data;
    }
    
    private void writeDataToUIGroup(Vector3 data, GameObject group) {
        foreach (TMP_InputField input in group.GetComponentsInChildren<TMP_InputField>()) {
            if (input.name == namePrefix + "x") {
                input.text = data.x.ToString();
            } else if (input.name == namePrefix + "y") {
                input.text = data.y.ToString();
            }
            else if (input.name == namePrefix + "z") {
                input.text = data.z.ToString();
            }
        }
    }

    private void writeDataToUI() {
        writeDataToUIGroup(tetMeshGO.transform.localPosition, positionGroup);
        writeDataToUIGroup(tetMeshGO.transform.localRotation.eulerAngles, rotationGroup);
        writeDataToUIGroup(tetMeshGO.transform.localScale, scaleGroup);
    }

    public void resetTransformation() {
        tetMeshGO.transform.localPosition = basePosition;
        tetMeshGO.transform.localRotation = Quaternion.Euler(baseRotation);
        tetMeshGO.transform.localScale = baseScale;
        writeDataToUI();
    }
}