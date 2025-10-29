using System;
using UnityEngine;

public class PlayerRotater : MonoBehaviour
{
    private InputSystem_Actions _actions;
    
    public float mousePitchSensitivity = 0.5f;
    public float mouseRollSensitivity = 0.5f;
    public float yawSpeed = 1f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _actions = new InputSystem_Actions();
        _actions.MainPlayer.Enable();
    }

    private void Update()
    {
        ControlFromMouse(_actions.MainPlayer.MouseControl.ReadValue<Vector2>());
        ControlFromAD(_actions.MainPlayer.ADAxis.ReadValue<float>());
    }

    void ControlFromAD(float value)
    {
        transform.Rotate(Vector3.up, value * yawSpeed * Time.deltaTime, Space.Self);
    }

    void ControlFromMouse(Vector2 vector)
    {
        transform.Rotate(Vector3.right, -vector.y * mousePitchSensitivity * Time.deltaTime, Space.Self);
        transform.Rotate(Vector3.forward, -vector.x * mouseRollSensitivity * Time.deltaTime, Space.Self);
    }
}
