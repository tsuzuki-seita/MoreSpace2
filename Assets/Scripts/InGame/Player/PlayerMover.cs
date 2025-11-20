using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using R3;

namespace MoreSpace.InGame.Player
{
    public class PlayerMover : MonoBehaviourPunCallbacks
    {

        private InputSystem_Actions _actions;
        [SerializeField] private Rigidbody rigid;
        [SerializeField] private float moveSpeed;
        private PlayerBuffs _buffs;
        private float finalSpeed;

        private void Start()
        {
            _actions = new InputSystem_Actions();
            _actions.MainPlayer.Enable();
            _buffs = GetComponent<PlayerBuffs>();
            finalSpeed = moveSpeed;
            _buffs.Speed.Subscribe(speedBonus =>
                {
                    finalSpeed = moveSpeed + speedBonus;
                    Debug.Log($"{finalSpeed} に更新");
                })
                .AddTo(this);
        }

        void Update()
        {
            if (!StartGameWithCountDown.isStartGame) return;

            if (_actions.MainPlayer.Move.ReadValue<float>() > 0)
            {
                Debug.Log(finalSpeed + "fianalSpeed");
                rigid.AddForce(finalSpeed * transform.forward, ForceMode.Acceleration);
            }
        }

        private void OnDestroy()
        {
            if (_actions == null) return;
            _actions.MainPlayer.Disable();
            _actions.Dispose();
        }
    }
}