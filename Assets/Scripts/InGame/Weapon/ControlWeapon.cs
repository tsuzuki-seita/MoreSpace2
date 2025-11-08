using System;
using System.Collections.Generic;
using UnityEngine;
using MoreSpace.InGame.Weapons;
using Photon.Pun;
using UnityEngine.InputSystem;

public class ControlWeapon : MonoBehaviourPunCallbacks
{
    [SerializeField] private int firstWeaponIndex = 0;
    [SerializeField] private List<Weapon> weapons = new();
    [SerializeField] private float ScrollThreshold = 0.01f;
    
    private Weapon nowWeapon;
    private InputSystem_Actions _actions;

    /// <summary>
    /// WeaponSkill の Initialize から呼び出される
    /// </summary>
    public void AddWeapon(Weapon newWeapon)
    {
        weapons ??= new List<Weapon>();
        weapons.Add(newWeapon);
        Debug.Log($"ControlWeapon: {newWeapon.GetType().Name} が追加されました。総数: {weapons.Count}");

        // もしこれが最初に追加された武器なら、自動で装備する
        if (photonView.IsMine && weapons.Count == 1)
        {
            photonView.RPC(nameof(ChangeWeapon), RpcTarget.All, firstWeaponIndex);
            ActivateInputs(); // 最初の武器が追加されたタイミングで入力を有効化
        }
    }

    private void Start()
    {
        if (!photonView.IsMine) return;
        photonView.RPC(nameof(ChangeWeapon), RpcTarget.All, firstWeaponIndex);
        ActivateInputs();
    }

    void ActivateInputs()
    {
        _actions = new InputSystem_Actions();
        _actions.MainPlayer.Enable();
        _actions.MainPlayer.Fire.started += OnFirePressed;
        _actions.MainPlayer.Fire.canceled += OnFireUp;
        _actions.MainPlayer.ChangeWeapon.started += OnScrollPerformed;
    }

    private void Update()
    {
        if (!photonView.IsMine) return;
        if (_actions.MainPlayer.Fire.IsPressed())
        {
            photonView.RPC(nameof(OnFireRPC), RpcTarget.All);
        }
    }

    private void OnFirePressed(InputAction.CallbackContext context)
    {
        photonView.RPC(nameof(OnFireDownRPC), RpcTarget.All);
    }
    private void OnFireUp(InputAction.CallbackContext context)
    {
        photonView.RPC(nameof(OnFireUpRPC), RpcTarget.All);
    }
    private void OnScrollPerformed(InputAction.CallbackContext context)
    {
        Vector2 scrollDelta = context.ReadValue<Vector2>();
        float scrollY = scrollDelta.y;
        Debug.Log($"Scroll Y: {scrollY}");
        if (scrollY > ScrollThreshold || scrollY < -ScrollThreshold)
        {
            int tesValue = nowWeapon is SingleShot ? 1 : 0;
            photonView.RPC(nameof(ChangeWeapon), RpcTarget.All, tesValue);
        }

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
            _actions.MainPlayer.ChangeWeapon.started -= OnScrollPerformed;
            _actions.MainPlayer.Disable();
            _actions.Dispose();
        }
    }
}


