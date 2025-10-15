// Domain/DTOs.cs
using System;

namespace MoreSpace.Domain
{
    [Serializable]
    public readonly struct SkillId
    {
        public readonly string Value;
        public SkillId(string value) => Value = value ?? string.Empty;
        public override string ToString() => Value;
    }

    [Serializable]
    public struct SkillLoadout
    {
        public SkillId Slot1;
        public SkillId Slot2;
        public SkillId Slot3;

        public SkillLoadout(SkillId s1, SkillId s2, SkillId s3)
        {
            Slot1 = s1; Slot2 = s2; Slot3 = s3;
        }

        public bool IsValid()
            => !string.IsNullOrEmpty(Slot1.Value)
            && !string.IsNullOrEmpty(Slot2.Value)
            && !string.IsNullOrEmpty(Slot3.Value);
    }

    // スキルセット、一回ロードしたら消える
    [Serializable]
    public class IngameArgs
    {
        public SkillLoadout Loadout;
        public IngameArgs(SkillLoadout loadout) => Loadout = loadout;
    }

    // 永続プロフィール
    [Serializable]
    public class UserProfile
    {
        public string UserName;
        public UserProfile(string userName) => UserName = userName ?? string.Empty;
    }
}
