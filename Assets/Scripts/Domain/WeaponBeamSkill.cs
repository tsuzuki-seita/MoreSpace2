using UnityEngine;
using MoreSpace.Domain;

[CreateAssetMenu(fileName = "NewWeaponBeamSkill", menuName = "MoreSpace/WeaponBeamSkill")]
public sealed class WeaponBeamSkill : WeaponSkill
{
    [Header("Weapon Data")]
    public float ReleaseDurationTime;

    public override void Initialize(GameObject owner)
    {
        owner.GetComponent<SkillViewer>().ActivateSkillUI(this, null);
        // 武器をプレイヤーにアタッチするなどの準備
        Debug.Log($"{SkillName} initialized on {owner.name}");
    }
}