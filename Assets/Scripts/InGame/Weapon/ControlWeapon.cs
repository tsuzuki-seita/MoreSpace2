using System;
using System.Collections.Generic;
using MoreSpace.InGame.Player;
using UnityEngine;
using MoreSpace.InGame.Weapons;
using NUnit.Framework;
using Photon.Pun;
using R3;
using UnityEngine.InputSystem;

public class ControlWeapon : MonoBehaviourPunCallbacks
{
    [SerializeField] private int firstWeaponIndex = 0;
    [SerializeField] private List<Weapon> weapons = new();
    [SerializeField] private float ScrollThreshold = 0.01f;

    private readonly ReactiveProperty<int> _nowIndex = new ReactiveProperty<int>(0); //現在装備している武器
    public ReadOnlyReactiveProperty<int> nowIndex => _nowIndex;
    private int usingIndex; //現在発火している武器
    private InputSystem_Actions _actions;

    //int toIndex, int weaponsCountの順で保持します
    private List<(int, int)> ChangeWeaponCache = new List<(int, int)>();

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
            photonView.RPC(nameof(ChangeWeapon), RpcTarget.All, firstWeaponIndex, weapons.Count);
            ActivateInputs(); // 最初の武器が追加されたタイミングで入力を有効化
        }
        
        CheckCache();
    }

    void CheckCache()
    {
        foreach (var cache in ChangeWeaponCache)
        {
            if (cache.Item2 == weapons.Count)
                ChangeWeapon(cache.Item1, cache.Item2);
            else
                break;
        }
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
        if (!photonView.IsMine || !StartGameWithCountDown.isStartGame) return;
        if (_actions.MainPlayer.Fire.IsPressed())
        {
            photonView.RPC(nameof(OnFireRPC), RpcTarget.All);
        }
    }

    private void OnFirePressed(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine || !StartGameWithCountDown.isStartGame) return;
        photonView.RPC(nameof(OnFireDownRPC), RpcTarget.All, _nowIndex.Value);
    }
    private void OnFireUp(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine || !StartGameWithCountDown.isStartGame) return;
        photonView.RPC(nameof(OnFireUpRPC), RpcTarget.All);
    }
    private void OnScrollPerformed(InputAction.CallbackContext context)
    {
        Vector2 scrollDelta = context.ReadValue<Vector2>();
        float scrollY = scrollDelta.y;
        if (scrollY > ScrollThreshold || scrollY < -ScrollThreshold)
        {
            int index = GetWrappedIndex(_nowIndex.Value + (int)Mathf.Sign(scrollY),weapons.Count);
            photonView.RPC(nameof(ChangeWeapon), RpcTarget.All, index, weapons.Count);
        }
    }
    
    private int GetWrappedIndex(int toIndex, int count)
    {
        if (count == 0) return 0;
        if (toIndex < 0) return count - 1;
        if (toIndex >= count) return 0;
        return toIndex;
    }

    [PunRPC]
    void OnFireDownRPC(int fireIndex)
    {
        usingIndex = fireIndex;
        weapons[usingIndex]?.OnFireDown();
    }
    [PunRPC]
    void OnFireRPC()
    {
        weapons[usingIndex]?.OnFire();
    }
    [PunRPC]
    void OnFireUpRPC()
    {
        weapons[usingIndex]?.OnFireUp();
    }

    [PunRPC]
    void ChangeWeapon(int toIndex, int weaponsCount)
    {
        Debug.Log($"OnChangeWeapon:{weapons.Count}");
        if (weapons.Count != weaponsCount)
        {
            ChangeWeaponCache.Add((toIndex,weaponsCount));
            return;
        }
        weapons[_nowIndex.Value]?.OnUnEquip();
        _nowIndex.Value = toIndex;
        weapons[_nowIndex.Value].OnEquip();
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


