using UnityEngine;
using MoreSpace.Domain;
using System;

[CreateAssetMenu(fileName = "NewPassiveSkill", menuName = "MoreSpace/Passive Skill")]
public sealed class PassiveSkill : Skill
{
    public enum PassiveKind
    {
        AttackUp,          // 全SingleShot.Damage に加算
        SpeedUp,           // PlayerMover.MoveSpeed に加算
        DefenseUp,         // PlayerHP.Defense に加算
        HandlingUp,        // PlayerRotator.YawSpeed に加算
        AttackForObjectUp  // 全SingleShot.ObjectDamage に加算
    }

    [Header("Passive Data")]
    public PassiveKind Kind;
    public float Value;

    public override void Initialize(GameObject owner)
    {
        if (owner == null) return;

        var buffs = owner.GetComponent<PlayerBuffs>();
        if (buffs == null) buffs = owner.AddComponent<PlayerBuffs>();

        buffs.Add(Kind, Value);
        owner.GetComponent<SkillViewer>().ActivateSkillUI(this, null);
        Debug.Log($"{SkillName} initialized (Passive): {Kind} +{Value} → {owner.name}");
    }
    
    // 軽量リフレクション: 指定コンポーネントの float プロパティ/フィールドに加算
    static void AddToFloatMember(GameObject owner, string componentTypeName, string memberName, float add)
    {
        var comp = owner.GetComponent(componentTypeName);
        if (comp == null) { Debug.LogWarning($"[Passive] {componentTypeName} が見つからないため {memberName} に加算できません。"); return; }

        var t = comp.GetType();
        var p = t.GetProperty(memberName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        if (p != null && (p.PropertyType == typeof(float) || p.PropertyType == typeof(double)))
        {
            var cur = Convert.ToSingle(p.GetValue(comp));
            p.SetValue(comp, cur + add);
            return;
        }
        var f = t.GetField(memberName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        if (f != null && (f.FieldType == typeof(float) || f.FieldType == typeof(double)))
        {
            var cur = Convert.ToSingle(f.GetValue(comp));
            f.SetValue(comp, cur + add);
            return;
        }
        Debug.LogWarning($"[Passive] {componentTypeName}.{memberName} が見つかりません。");
    }
}