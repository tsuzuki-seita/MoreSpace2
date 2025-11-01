using System;
using System.Collections.Generic;
using MoreSpace.Application;
using UnityEngine;

namespace MoreSpace.Domain
{
    // ユーザー定義
    public enum SkillType 
    { 
        // Passive
        HandlingUp, 
        MoveSpeedUp, 
        MaxHpUp,
        
        // Weapon
        Laser, 
        Homing, 
        HeavyShot, 
        
        // Active/Buff
        Stealth, 
        Missile, // (これはWeaponでは？ SkillTypeの分類は自由です)
        Sensor 
    }

    // ユーザー定義
    public enum DeckLevel { Level1 = 1, Level2 = 2, Level3 = 3 }

    // スキルデータの基底クラス
    public abstract class Skill : ScriptableObject
    {
        [Header("Common Data")]
        public SkillType Type;
        public DeckLevel Level; // このスキルが属するレベル
        public string SkillName;
        [TextArea] public string Description;
        public Sprite Icon;

        // スキル選択時に呼ばれる初期化処理など
        public abstract void Initialize(GameObject owner);

        // スキルの実処理（パッシブなら空、アクティブなら発動）
        public abstract void Act(GameObject owner);
    }

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

        public override void Act(GameObject owner)
        {
            // パッシブなので発動(Act)処理は通常不要
        }
    }

    // ウェポンやバフなど、能動的に発動するスキルの基底
    public abstract class ActiveSkill : Skill
    {
        [Header("Active Data")]
        public float RecastTime; // リキャストタイム
    }

    [CreateAssetMenu(fileName = "NewWeaponSkill", menuName = "MoreSpace/Weapon Skill")]
    public sealed class WeaponSkill : ActiveSkill
    {
        [Header("Weapon Data")]
        public float Distance;
        public float Damage;
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

    [Serializable]
    public class SkillSet
    {
        public Skill Level1Skill;
        public Skill Level2Skill;
        public Skill Level3Skill;

        public SkillSet(Skill s1, Skill s2, Skill s3)
        {
            Level1Skill = s1;
            Level2Skill = s2;
            Level3Skill = s3;
        }
    }
    
    // IngameSceneManagerの例 (IngameArgs) に倣った一時引数クラス
    public sealed class StartIngameArgs : ITransientArgs
    {
        public readonly SkillSet SelectedSkills;

        public StartIngameArgs(SkillSet skills)
        {
            SelectedSkills = skills;
        }
    }
}
