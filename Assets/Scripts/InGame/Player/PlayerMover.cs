using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using R3;

namespace MoreSpace.InGame.Player
{
    public class PlayerMover : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Rigidbody rigid;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float acceleration;
        private PlayerBuffs _buffs;
        private float finalSpeed;

        private void Start()
        {
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
            if (!photonView.IsMine || !StartGameWithCountDown.isStartGame) return;

            rigid.AddForce(acceleration * transform.forward, ForceMode.Acceleration);

            if (rigid.linearVelocity.magnitude > finalSpeed)
            {
                rigid.linearVelocity = rigid.linearVelocity.normalized * finalSpeed;
            }
        }
    }
}