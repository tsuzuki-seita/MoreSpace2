using UnityEngine;
using MoreSpace.Domain;
using MoreSpace.InGame.Weapons;
using MoreSpace.InGame.Weapons.Bullets;

[CreateAssetMenu(fileName = "NewWeaponSkill", menuName = "MoreSpace/Weapon Skill")]
public class WeaponSkill : Skill
{
    [Header("Weapon Data")]
    public float Distance;
    public float Damage;
    public float RecastTime;
    public float Speed;

    [Header("Bullet Prefab")]
    public SingleBullet ProjectilePrefab;
    
    public override void Initialize(GameObject owner)
    {
        Debug.Log(owner.name);
        // SingleShot を owner に付与
        var shot = owner.AddComponent<MoreSpace.InGame.Weapons.SingleShot>();

        // ★ 直代入（public化したのでリフレクション不要）
        shot.bullet             = ProjectilePrefab;
        shot.speed              = Speed;
        shot.damage             = Mathf.RoundToInt(Damage);

        // ControlWeapon へレベル順で登録
        var cw = owner.GetComponent<ControlWeapon>();
        if (cw != null) cw.AddWeapon(shot);

        Debug.Log($"{SkillName} initialized on {owner.name} (Dmg={Damage}, Spd={Speed}, Dist={Distance})");
    }
}