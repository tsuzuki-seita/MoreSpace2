using UnityEngine;
using static PassiveSkill;
using R3;

[DisallowMultipleComponent]
public sealed class PlayerBuffs : MonoBehaviour
{
    [Header("Additive buffs (accumulated)")]
    public float Attack;          // 例: 通常ダメージに加算
    public float Defense;         // 例: 受けるダメージを軽減など
    // public float Speed;           // 例: 移動速度に加算
    public readonly ReactiveProperty<float> Speed = new ReactiveProperty<float>(0f);
    public float Handling;        // 例: 旋回速度に加算
    public float AttackForObject; // 例: オブジェクト専用ダメージに加算

    /// <summary>パッシブの値を種類に応じて加算</summary>
    public void Add(PassiveKind kind, float value)
    {
        switch (kind)
        {
            case PassiveKind.AttackUp:          Attack          += value; break;
            case PassiveKind.DefenseUp:         Defense         += value; break;
            case PassiveKind.SpeedUp:           Speed.Value     += value; break;
            case PassiveKind.HandlingUp:        Handling        += value; break;
            case PassiveKind.AttackForObjectUp: AttackForObject += value; break;
            default: break;
        }
    }

    /// <summary>開始時に全部リセットしたい場合に使用（任意）</summary>
    public void ClearAll()
    {
        Attack = Defense = Handling = AttackForObject = 0f;
        Speed.Value = 0f;
    }
}