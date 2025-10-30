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
    }

    private void Update()
    {
        // if (_actions.MainPlayer.ChangeWeapon.ReadValue<bool>()) ChangeWeapon(nowWeapon is SingleShot ? 1 : 0);
        if (_actions.MainPlayer.Fire.IsPressed())
        {
            nowWeapon?.OnFireDown();
            Debug.Log("Fire Down");
        }
        
        if (_actions.MainPlayer.Fire.ReadValue<bool>()) nowWeapon?.OnFire();
        if (_actions.MainPlayer.Fire.ReadValue<bool>()) nowWeapon?.OnFireUp();
    }



    void ChangeWeapon(int toIndex)
    {
        nowWeapon?.OnUnEquip();
        nowWeapon = weapons[toIndex];
        nowWeapon.OnEquip();
    }
}


