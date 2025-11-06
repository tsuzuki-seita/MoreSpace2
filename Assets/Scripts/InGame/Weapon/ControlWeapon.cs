using System;
using System.Collections.Generic;
using UnityEngine;
using MoreSpace.InGame.Weapons;
using Photon.Pun;
using UnityEngine.InputSystem;

public class ControlWeapon : MonoBehaviourPunCallbacks
{
    [SerializeField] private int firstWeaponIndex = 0;
    [SerializeField] private List<Weapon> weapons;
    private Weapon nowWeapon;
    private InputSystem_Actions _actions;

    private void Start()
    {
        if(!photonView.IsMine)return;
        photonView.RPC(nameof(ChangeWeapon),RpcTarget.All,firstWeaponIndex);
        ActivateInputs();
    }

    void ActivateInputs()
    {
        _actions = new InputSystem_Actions();
        _actions.MainPlayer.Enable();
        _actions.MainPlayer.Fire.started += OnFirePressed;
        _actions.MainPlayer.Fire.canceled += OnFireUp;
        _actions.MainPlayer.ChangeWeapon.started += OnCPressed;
    }

    private void Update()
    {
        if(!photonView.IsMine)return;
        if (_actions.MainPlayer.Fire.IsPressed())
        {
            photonView.RPC(nameof(OnFireRPC),RpcTarget.AllBuffered);
        }
    }

    private void OnFirePressed(InputAction.CallbackContext context)
    {
        photonView.RPC(nameof(OnFireDownRPC),RpcTarget.AllBuffered);
    }
    private void OnFireUp(InputAction.CallbackContext context)
    {
        photonView.RPC(nameof(OnFireUpRPC),RpcTarget.AllBuffered);
    }
    private void OnCPressed(InputAction.CallbackContext context)
    {
        int tesValue = nowWeapon is SingleShot ? 1 : 0;
        photonView.RPC(nameof(ChangeWeapon),RpcTarget.All,tesValue);
    }

    [PunRPC]
    void OnFireDownRPC()
    {
        nowWeapon?.OnFireDown();
    }
    [PunRPC]
    void OnFireRPC()
    {
        nowWeapon?.OnFire();
    }
    [PunRPC]
    void OnFireUpRPC()
    {
        nowWeapon?.OnFireUp();
    }
    
    [PunRPC]
    void ChangeWeapon(int toIndex)
    {
        nowWeapon?.OnUnEquip();
        nowWeapon = weapons[toIndex];
        nowWeapon.OnEquip();
    }

    private void OnDestroy()
    {
        if (photonView.IsMine && _actions != null)
        {
            _actions.MainPlayer.Fire.started -= OnFirePressed;
            _actions.MainPlayer.Fire.canceled -= OnFireUp;
            _actions.MainPlayer.ChangeWeapon.started -= OnCPressed;
            _actions.MainPlayer.Disable();
            _actions.Dispose();
        }
    }
}


