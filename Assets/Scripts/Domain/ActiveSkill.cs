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
        // Active を owner に付与
        var active = owner.AddComponent<MoreSpace.InGame.Weapons.Active>();

        // ★ 直代入（public化したのでリフレクション不要）
        active.RecastTime = RecastTime;
        active.Duration   = Duration;

        // ControlWeapon へレベル順で登録
        var cw = owner.GetComponent<ControlWeapon>();
        if (cw != null) cw.AddWeapon(active);

        Debug.Log($"{SkillName} initialized on {owner.name} (Recast={RecastTime}, Duration={Duration})");
    }
}