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

    [SerializeField] private float radius = 5f;
    [Range(0f, 180f)]
    [SerializeField] private const float verticalRotation = 70f;
    [Range(-180, 180f)]
    [SerializeField] private float horizentalRotation = 0f;

    [SerializeField] private float rotationSpeed = 1f;
    private float prevValue;
    private float elapsedRotation;

    private readonly float sinVertical = Mathf.Sin(Mathf.Deg2Rad * verticalRotation);
    private readonly float cosVertical = Mathf.Cos(Mathf.Deg2Rad * verticalRotation);

    private void Start()
    {
        if (targetTransfrom == null)
            this.enabled = false;
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
            if ((elapsedRotation > 180f || elapsedRotation < -180f) == false)
            {
                horizentalRotation += prevValue * rotationSpeed;
            }
        }
        else
        {
            elapsedRotation = 0f;
        }

        RepositionCamera();
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
        Debug.Log(targetTransfrom.position);

        transform.position = targetTransfrom.position + new Vector3(
            radius * sinVertical * Mathf.Sin(horizentalRotation * Mathf.Deg2Rad),
            radius * cosVertical,
            radius * sinVertical * Mathf.Cos(horizentalRotation * Mathf.Deg2Rad));
        transform.LookAt(targetTransfrom);
    }

    private void DoCameraRotation(InputAction.CallbackContext obj)
    {
        prevValue = obj.ReadValue<float>();
    }
}
