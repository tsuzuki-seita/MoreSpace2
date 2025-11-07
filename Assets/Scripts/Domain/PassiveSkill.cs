using UnityEngine;
using MoreSpace.Domain;

[CreateAssetMenu(fileName = "NewPassiveSkill", menuName = "MoreSpace/Passive Skill")]
public sealed class PassiveSkill : Skill
{
    [Header("Passive Data")]
    public float Value; // ハンドリング向上値、移動速度UP値など

    public override void Initialize(GameObject owner)
    {
        // 例: プレイヤーのステータスにValueを適用する
        Debug.Log($"{SkillName} initialized: Applying value {Value} to {owner.name}");
        // owner.GetComponent<PlayerStats>().MoveSpeed += Value;
    }
}