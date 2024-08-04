using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defines;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Animations;
using UnityEngine.InputSystem.Controls;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform targetTransfrom;

    [SerializeField] private float radios = 5f;
    [Range(0f, 180f)]
    [SerializeField] private float verticalRotation = 70f;
    [Range(-180, 180f)]
    [SerializeField] private float horizentalRotation = 0f;

    [SerializeField] private float rotationSpeed = 1f;
    private float prevValue;
    private float elapsedRotation;

    private void Start()
    {
        if (targetTransfrom == null)
            this.enabled = false;
        transform.parent = targetTransfrom;
        RepositionCamera();

    }

    private void OnEnable()
    {
        InputManager.Instance.AddInputEventFunction(
            new InputDefines.InputActionName(InputDefines.ActionMapType.PlayerActions, InputDefines.CameraRotation),
            InputDefines.ActionPoint.All,
            DoCameraRotation
            );
    }
    private void LateUpdate()
    {
        if(prevValue != 0f)
        {
            elapsedRotation += prevValue * rotationSpeed;
            if (elapsedRotation > 180f || elapsedRotation < -180f)
                return;

            horizentalRotation += prevValue * rotationSpeed;
            RepositionCamera();
        }
        else
        {
            elapsedRotation = 0f;
        }
    }

    private void OnDisable()
    {
        InputManager.Instance.RemoveInputEventFunction(
            new InputDefines.InputActionName(InputDefines.ActionMapType.PlayerActions, InputDefines.CameraRotation),
            InputDefines.ActionPoint.All,
            DoCameraRotation
            );
    }

    private void RepositionCamera()
    {
        transform.position = new Vector3(
            radios * Mathf.Sin(Mathf.Deg2Rad * verticalRotation) * Mathf.Sin(horizentalRotation * Mathf.Deg2Rad),
            radios * Mathf.Cos(Mathf.Deg2Rad * verticalRotation),
            radios * Mathf.Sin(Mathf.Deg2Rad * verticalRotation) * Mathf.Cos(horizentalRotation * Mathf.Deg2Rad));
        transform.LookAt(targetTransfrom);
    }

    private void DoCameraRotation(InputAction.CallbackContext obj)
    {
        prevValue = obj.ReadValue<float>();
    }
}
