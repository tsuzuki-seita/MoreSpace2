using System.Collections.Generic;
using System.Linq;
using MoreSpace.Application;
using MoreSpace.Domain;
using UnityEngine;

namespace MoreSpace.Infrastructure
{
    public sealed class ResourceSkillRepository : ISkillRepository
    {
        private List<Skill> _skillCache;
        private const string SkillResourcePath = "Skills"; // Resources/Skills フォルダを想定

        public IEnumerable<Skill> GetAllSkills()
        {
            if (_skillCache == null)
            {
                // Resources/Skills フォルダ配下にある全ての Skill 型 (と
                // その派生型) の ScriptableObject を読み込む
                _skillCache = Resources.LoadAll<Skill>(SkillResourcePath).ToList();
            }
            return _skillCache;
        }

        public IEnumerable<Skill> GetSkillsByLevel(DeckLevel level)
        {
            return GetAllSkills().Where(skill => skill.Level == level);
        }
    }
}