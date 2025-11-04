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
        // if (_actions.MainPlayer.ChangeWeapon.ReadValue<bool>()) ChangeWeapon(nowWeapon is SingleShot ? 1 : 0);


        if (_actions.MainPlayer.Fire.IsPressed())
        {
            nowWeapon?.OnFire();
            // Debug.Log("押され続けてる時");
        }

    }

    private void OnFirePressed(InputAction.CallbackContext context)
    {
        nowWeapon?.OnFireDown();
        // Debug.Log("ボタンが押されました！");
        // if (_actions.MainPlayer.Fire.IsPressed())
        // {
        //     nowWeapon?.OnFireDown();
        //     Debug.Log("Fire Down");
        // }ここの部分
    }

    private void OnFireUp(InputAction.CallbackContext context)
    {
        nowWeapon?.OnFireUp();
        // Debug.Log("ボタンが離されました！ (canceled)");
        // if (_actions.MainPlayer.Fire.ReadValue<bool>()) nowWeapon?.OnFireUp();
        // ここの部分
    }
    private void OnCPressed(InputAction.CallbackContext context)
    {
        ChangeWeapon(nowWeapon is SingleShot ? 1 : 0);
        Debug.Log("Cキーが押されました！");
        
    }



    void ChangeWeapon(int toIndex)
    {
        nowWeapon?.OnUnEquip();
        nowWeapon = weapons[toIndex];
        nowWeapon.OnEquip();
    }
}


