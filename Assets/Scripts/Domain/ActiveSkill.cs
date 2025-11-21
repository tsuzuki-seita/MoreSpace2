using UnityEngine;
using MoreSpace.Domain;
using MoreSpace.InGame.Weapons;

[CreateAssetMenu(fileName = "NewActiveSkill", menuName = "MoreSpace/Active Skill")]
public sealed class ActiveSkill : Skill
{
    [Header("Active Data")]
    public float RecastTime; // リキャストタイム
    public float Duration;   // 効果持続時間（バフ系の場合）

    public enum ActiveSkillType
    {
        Stealth,
        Unbreakable,
    }

    [Header("Effect Type")]
    public ActiveSkillType SkillType; // このデータが Stealth 用なのか Unbreakable 用なのか

    public override void Initialize(GameObject owner)
    {
        if (owner == null)
        {
            Debug.LogError("[ActiveSkill] owner が null です");
            return;
        }

        // どの Active 派生クラスを使うかを決定
        Active active = null;

        switch (SkillType)
        {
            case ActiveSkillType.Stealth:
                active = owner.GetComponent<Stealth>();
                if (active == null) active = owner.AddComponent<Stealth>();
                break;

            case ActiveSkillType.Unbreakable:
                active = owner.GetComponent<Unbreakable>();
                if (active == null) active = owner.AddComponent<Unbreakable>();
                break;

            default:
                Debug.LogError($"[ActiveSkill] 未対応の SkillType: {SkillType}");
                return;
        }

        // ScriptableObject 側の値を反映
        active.RecastTime = RecastTime;
        active.Duration   = Duration;

        // ControlWeapon に登録
        var cw = owner.GetComponent<ControlWeapon>();
        if (cw == null)
        {
            Debug.LogError("[ActiveSkill] owner に ControlWeapon が見つかりません");
            return;
        }

        cw.AddWeapon(active);

        Debug.Log(
            $"{SkillName} initialized on {owner.name} " +
            $"(Type={SkillType}, Recast={RecastTime}, Duration={Duration})"
        );
    }
}