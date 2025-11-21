using UnityEngine;
using static PassiveSkill;
using R3;

[DisallowMultipleComponent]
public sealed class PlayerBuffs : MonoBehaviour
{
    [Header("Additive buffs (accumulated)")]     
    public readonly ReactiveProperty<float> Attack = new ReactiveProperty<float>(0f);
    public float Defense;         // 例: 受けるダメージを軽減など        
    public readonly ReactiveProperty<float> Speed = new ReactiveProperty<float>(0f);    // 例: 移動速度に加算 
    public readonly ReactiveProperty<float> Handling = new ReactiveProperty<float>(0f); // 例: 旋回速度に加算
    public readonly ReactiveProperty<float> AttackForObject = new ReactiveProperty<float>(0f); // 例: オブジェクト専用ダメージに加算

    /// <summary>パッシブの値を種類に応じて加算</summary>
    public void Add(PassiveKind kind, float value)
    {
        switch (kind)
        {
            case PassiveKind.AttackUp: Attack.Value += value; break;
            case PassiveKind.DefenseUp: Defense.Value += value; break;
            case PassiveKind.SpeedUp: Speed.Value += value; break;
            case PassiveKind.HandlingUp: Handling.Value += value; break;
            case PassiveKind.AttackForObjectUp: AttackForObject.Value += value; break;
            default: break;
        }
    }

    /// <summary>開始時に全部リセットしたい場合に使用（任意）</summary>
    public void ClearAll()
    {
        Defense = 0f;
        Attack.Value = Speed.Value = AttackForObject.Value = Handling.Value=0f;
    }
}