using System;
using MoreSpace.InGame;
using MoreSpace.InGame.Player;
using UnityEngine;
using UnityEngine.UI;

public class SetHpToUI : MonoBehaviour
{
    [SerializeField] private Scrollbar hpBar;
    [SerializeField] private HealthBase target;
    [SerializeField] private bool isLookPlayer;
    void Start()
    {
        if(isLookPlayer)
            FindAnyObjectByType<LookUiToCamera>().AssertUI(hpBar.transform);
        target.OnDamage += ChangeValue;
    }

    private void OnDestroy()
    {
        target.OnDamage -= ChangeValue;
    }

    void ChangeValue(int hp, int maxHp)
    {
        hpBar.size = (float)hp / maxHp;
    }
}
