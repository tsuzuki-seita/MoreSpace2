using UnityEngine;
using MoreSpace.InGame.Weapons.Bullets;

namespace MoreSpace.InGame.Weapons
{
    [CreateAssetMenu(fileName = "NewWeaponBeamSkill", menuName = "MoreSpace/WeaponBeamSkill")]
    public sealed class WeaponBeamSkill : WeaponSkill
    {
        [Header("Weapon Data")] public float ReleaseDurationTime;

        public override void Initialize(GameObject owner)
        {
            var laser = owner.AddComponent<MoreSpace.InGame.Weapons.Beam>();

            laser.bulletPrefab = ProjectilePrefab.GetComponent<BeamBullet>();
            laser.maxDistance = Distance;
            laser.damage = Mathf.RoundToInt(Damage);
            laser.fireRate = RecastTime;
            laser.maxBeamDuration = ReleaseDurationTime;

            // ControlWeapon へレベル順で登録
            var cw = owner.GetComponent<ControlWeapon>();
            if (cw != null) cw.AddWeapon(laser);

            owner.GetComponent<SkillViewer>().ActivateSkillUI(this, laser);
            Debug.Log($"{SkillName} initialized on {owner.name}");
        }
    }
}