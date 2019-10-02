using UnityEngine;

public enum ECamName {
    CAM_FRONT,
    CAM_BACK,
    CAM_WORLD,
    CAM_LEFT,
    CAM_RIGHT
}

public class CameraController : MonoBehaviour {
    [SerializeField] private ECamName startCam;

    [SerializeField] private Camera worldCam;
    [SerializeField] private Camera carFrontCam;
    [SerializeField] private Camera carRightCam;
    [SerializeField] private Camera carLeftCam;
    [SerializeField] private Camera carBackCam;


    private void Start() {
        activateCam(startCam);
    }

    private void activateCam(ECamName camName) {
        disableAllCams();
        switch (camName) {
            case ECamName.CAM_WORLD: worldCam.enabled = true; break;
            case ECamName.CAM_FRONT: carFrontCam.enabled = true; break;
            case ECamName.CAM_LEFT: carLeftCam.enabled = true; break;
            case ECamName.CAM_BACK: carBackCam.enabled = true; break;
            case ECamName.CAM_RIGHT: carRightCam.enabled = true; break;
        }
    }

    private void disableAllCams() {
        worldCam.enabled = false;
        carFrontCam.enabled = false;
        carRightCam.enabled = false;
        carLeftCam.enabled = false;
        carBackCam.enabled = false;
    }

    void Update () {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            activateCam(ECamName.CAM_BACK);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            activateCam(ECamName.CAM_FRONT);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            activateCam(ECamName.CAM_RIGHT);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            activateCam(ECamName.CAM_LEFT);
        if (Input.GetKeyDown(KeyCode.Alpha0))
            activateCam(ECamName.CAM_WORLD);
    }
}
