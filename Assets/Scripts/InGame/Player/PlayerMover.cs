using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMover : MonoBehaviourPunCallbacks
{
    
    private InputSystem_Actions _actions;
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private float moveSpeed;

    private void Start()
    {
        _actions = new InputSystem_Actions();
        _actions.MainPlayer.Enable();
    }

    void Update()
    {
        if (_actions.MainPlayer.Move.ReadValue<float>() > 0)
        {
            rigid.AddForce(moveSpeed * transform.forward, ForceMode.Acceleration);
        }
    }
}