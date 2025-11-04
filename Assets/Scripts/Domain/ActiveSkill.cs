using UnityEngine;
using MoreSpace.Domain;

[CreateAssetMenu(fileName = "NewActiveSkill", menuName = "MoreSpace/Active Skill")]
public sealed class ActiveSkill : Skill
{
    [Header("Active Data")]
    public float RecastTime; // リキャストタイム
    public float Duration;   // 効果持続時間（バフ系の場合）

    public override void Initialize(GameObject owner)
    {
        // 例: プレイヤーのステータスにValueを適用する
        Debug.Log($"{SkillName} initialized: Time: {Duration} to {owner.name}");
        // owner.GetComponent<PlayerStats>().MoveSpeed += Value;
    }

    public override void Act(GameObject owner)
    {
        // パッシブなので発動(Act)処理は通常不要
    }
}