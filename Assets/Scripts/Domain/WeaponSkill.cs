using UnityEngine;
using MoreSpace.Domain;

[CreateAssetMenu(fileName = "NewWeaponSkill", menuName = "MoreSpace/Weapon Skill")]
public sealed class WeaponSkill : Skill
{
    [Header("Weapon Data")]
    public float Distance;
    public float Damage;
    public float RecastTime;
    // public GameObject ProjectilePrefab; // プレハブなど

    public override void Initialize(GameObject owner)
    {
        // 武器をプレイヤーにアタッチするなどの準備
        Debug.Log($"{SkillName} initialized on {owner.name}");
    }

    public override void Act(GameObject owner)
    {
        // 武器を発射する処理
        Debug.Log($"{SkillName} Act! Damage: {Damage}, Recast: {RecastTime}");
    }
}