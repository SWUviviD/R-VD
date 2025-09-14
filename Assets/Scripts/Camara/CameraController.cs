using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoSingleton<CameraController>
{
    public enum CameraMode
    {
        Fixed,
        Cinematography,
        Orbit,
        Dialog,
    }

    [SerializeField] private Camera currentCamera;

    [field: SerializeField] public Camera MainCamera { get; private set; }
    [field: SerializeField] public Camera DialogueCamera { get; private set; }

    [Header("Scripts")]
    [SerializeField] private OrbitCamera _orbitCam;

    private CameraMode _mode = CameraMode.Orbit;

    public CameraEffector GetCameraEffector => currentCamera?.GetComponent<CameraEffector>();

    public void OnLoadCameraSetting(Transform cam)
    {
        MainCamera = cam.GetComponent<Camera>();
        currentCamera = MainCamera;

        _orbitCam = cam.GetComponent<OrbitCamera>();
    }

    public void SetDialogueCamera(Camera cam)
    {
        DialogueCamera = cam;
        DialogueCamera.gameObject.AddComponent<CameraEffector>();
    }

    public void SetCameraMode(CameraMode mode)
    {
        if (mode == _mode)
            return;

        currentCamera?.gameObject.SetActive(false);
        currentCamera.tag = "Camera";
        _mode = mode;

        switch (mode)
        {
            case CameraMode.Fixed:
                {
                    currentCamera = MainCamera;
                    _orbitCam.enabled = false;
                    break;
                }
            case CameraMode.Orbit:
                {
                    currentCamera = MainCamera;
                    _orbitCam.enabled = true;
                    break;
                }
            case CameraMode.Dialog:
                {
                    currentCamera = DialogueCamera;
                    _orbitCam.enabled = false;
                    break;
                }
        }

        currentCamera.gameObject.SetActive(true);
        currentCamera.tag = "MainCamera";
    }

    public void SetCameraPositionAndRotation(Vector3 rotation, Vector3 position)
    {
        switch (_mode)
        {
            case CameraMode.Fixed:
            case CameraMode.Cinematography:
                {
                    currentCamera.transform.position = position;
                    currentCamera.transform.rotation = Quaternion.Euler(rotation);
                    break;
                }

            case CameraMode.Orbit:
                {
                    _orbitCam.SetCameraRotation(rotation.x);
                    break;
                }

            default: break;
        }
    }
}