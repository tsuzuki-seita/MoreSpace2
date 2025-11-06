using System;
using System.Collections.Generic;
using UnityEngine;
using MoreSpace.InGame.Weapons;
using Photon.Pun;
using UnityEngine.InputSystem;

public class ControlWeapon : MonoBehaviour
{
    [SerializeField] private int firstWeaponIndex = 0;
    [SerializeField] private List<Weapon> weapons;
    private Weapon nowWeapon;
    private InputSystem_Actions _actions;

    private void Start()
    {
        ChangeWeapon(firstWeaponIndex);
        _actions = new InputSystem_Actions();
        _actions.MainPlayer.Enable();
        _actions.MainPlayer.Fire.started += OnFirePressed;
        _actions.MainPlayer.Fire.canceled += OnFireUp;
        _actions.MainPlayer.ChangeWeapon.started += OnCPressed;
    }

    private void Update()
    {


        if (_actions.MainPlayer.Fire.IsPressed())
        {
            nowWeapon?.OnFire();
        }

    }

    private void OnFirePressed(InputAction.CallbackContext context)
    {
        nowWeapon?.OnFireDown();
    }

    private void OnFireUp(InputAction.CallbackContext context)
    {
        nowWeapon?.OnFireUp();
    }
    private void OnCPressed(InputAction.CallbackContext context)
    {
        ChangeWeapon(nowWeapon is SingleShot ? 1 : 0);
        
    }



    void ChangeWeapon(int toIndex)
    {
        nowWeapon?.OnUnEquip();
        nowWeapon = weapons[toIndex];
        nowWeapon.OnEquip();
    }
}


