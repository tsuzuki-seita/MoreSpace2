using System;
using UnityEngine;
using R3;

namespace MoreSpace.InGame.Player
{

    public class PlayerRotater : MonoBehaviour
    {
        private InputSystem_Actions _actions;
    
        public float mousePitchSensitivity = 0.5f;
        public float mouseRollSensitivity = 0.5f;
        public float yawSpeed = 1f;
        private PlayerBuffs _buffs;
        private float rotateBuff =0;
        private float _finalYawSpeed;
        private float _finalMousePitchSensitivity;

        void Start()
        {
            _actions = new InputSystem_Actions();
            _actions.MainPlayer.Enable();
            _buffs = GetComponent<PlayerBuffs>();
            _finalYawSpeed = yawSpeed;
            _finalMousePitchSensitivity = mousePitchSensitivity;
            _buffs.Handling.Subscribe(handlingBonus => 
                {  
                    // マウス感度変えるならここここら辺いじってもらえると。。
                    // _finalMousePitchSensitivity = mousePitchSensitivity + handlingBonus;
                    _finalYawSpeed = yawSpeed + handlingBonus;
                     Debug.Log($"回転バフ{rotateBuff} に更新");
                })
                .AddTo(this);
        }
        
        private void Update()
        {
            if (!StartGameWithCountDown.isStartGame) return;

            ControlFromMouse(_actions.MainPlayer.MouseControl.ReadValue<Vector2>());
            ControlFromAD(_actions.MainPlayer.ADAxis.ReadValue<float>());
        }

        void ControlFromAD(float value)
        {
            transform.Rotate(Vector3.forward, value * _finalYawSpeed * Time.deltaTime, Space.Self);
        }

        void ControlFromMouse(Vector2 vector)
        {
            transform.Rotate(Vector3.right, -vector.y * _finalMousePitchSensitivity * Time.deltaTime, Space.Self);
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