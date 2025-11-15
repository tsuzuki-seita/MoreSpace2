using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMover : MonoBehaviourPunCallbacks
{
    
    private InputSystem_Actions _actions;
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private float moveSpeed;
    private PlayerBuffs _buffs;

    private void Start()
    {
        _actions = new InputSystem_Actions();
        _actions.MainPlayer.Enable();
    }

    void Update()
    {
        if (_actions.MainPlayer.Move.ReadValue<float>() > 0)
        {
            // _buffs = _buffs ?? GetComponent<PlayerBuffs>();
            float speedBonus    = _buffs != null ? _buffs.Speed  : 0.0f;
            float finalSpeed = moveSpeed + speedBonus;
            Debug.Log("Speed Bonus: " + speedBonus);
            rigid.AddForce(finalSpeed* transform.forward, ForceMode.Acceleration);
        }
    }

    private void OnDestroy()
    {
        if(_actions == null) return;
        _actions.MainPlayer.Disable();
        _actions.Dispose();
    }
}