using UnityEngine;
using MoreSpace.InGame.Weapons;
using MoreSpace.InGame.Weapons.Bullets;

[CreateAssetMenu(fileName = "NewWeaponSkill", menuName = "MoreSpace/Weapon Missile Skill")]
public class WeaponMissileSkill : WeaponSkill
{
    public bool IsHomingAlways = false;
    public override void Initialize(GameObject owner)
    {
        Debug.Log(owner.name);
        // SingleShot を owner に付与
        var shot = owner.AddComponent<MissileShot>();

        // ★ 直代入（public化したのでリフレクション不要）
        shot.bullet = ProjectilePrefab.GetComponent<HomingBullet>();
        shot.Distance = Distance;
        shot.speed = Speed;
        shot.damage = Mathf.RoundToInt(Damage);
        shot.fireRate = RecastTime;
        shot.isHomingAlways = IsHomingAlways;

        // ControlWeapon へレベル順で登録
        var cw = owner.GetComponent<ControlWeapon>();
        if (cw != null) cw.AddWeapon(shot);

        owner.GetComponent<SkillViewer>().ActivateSkillUI(this, shot);
        Debug.Log($"{SkillName} initialized on {owner.name} (Dmg={Damage}, Spd={Speed}, Dist={Distance})");
    }
}