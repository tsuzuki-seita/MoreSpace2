using System;
using UnityEngine;

namespace MoreSpace.InGame.Player
{
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
            if (!StartGameWithCountDown.isStartGame) return;

            ControlFromMouse(_actions.MainPlayer.MouseControl.ReadValue<Vector2>());
            ControlFromAD(_actions.MainPlayer.ADAxis.ReadValue<float>());
        }

        void ControlFromAD(float value)
        {
            transform.Rotate(Vector3.forward, value * yawSpeed * Time.deltaTime, Space.Self);
        }

        void ControlFromMouse(Vector2 vector)
        {
            transform.Rotate(Vector3.right, -vector.y * mousePitchSensitivity * Time.deltaTime, Space.Self);
            transform.Rotate(Vector3.up, vector.x * mouseRollSensitivity * Time.deltaTime, Space.Self);
        }

        private void OnDestroy()
        {
            if (_actions == null) return;
            _actions.MainPlayer.Disable();
            _actions.Dispose();
        }
    }
}