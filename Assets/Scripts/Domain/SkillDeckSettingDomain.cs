using System;
using System.Collections.Generic;
using MoreSpace.Application;
using UnityEngine;

namespace MoreSpace.Domain
{
    // ユーザー定義
    public enum SkillType
    {
        // Weapon
        Normal,
        Rapid,
        Laser,
        Homing,
        Missile,

        // Passive
        AttackUp,
        SpeedUp,
        DefenseUp,
        HandlingUp,
        AttackForObjectUp,

        AttackUp2,
        SpeedUp2,
        DefenseUp2,
        HandlingUp2,
        AttackForObjectUp2,

        AttackUp3,
        SpeedUp3,
        DefenseUp3,
        HandlingUp3,
        AttackForObjectUp3,

        HPHeal,

        // Active/Buff
        Unbeatable,
        Stealth,
        Sensor,
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
