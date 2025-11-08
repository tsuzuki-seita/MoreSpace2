// Infrastructure/ResourceSkillRepository.cs
using System.Collections.Generic;
using System.Linq;
using MoreSpace.Application;
using MoreSpace.Domain;
using UnityEngine;

namespace MoreSpace.Infrastructure
{
    public class ResourceSkillRepository : ISkillRepository
    {
        public static Dictionary<string, Skill> Skills = new Dictionary<string, Skill>();
        private List<Skill> _skillCache;

        // ★変更点 1: 検索対象のフォルダパスを配列で定義
        // (パスは "Resources" フォルダからの相対パスです)
        private readonly string[] _skillResourcePaths = 
        {
            "SkillDates/Active",
            "SkillDates/Passive",
            "SkillDates/Weapon"
        };

        public IEnumerable<Skill> GetAllSkills()
        {
            if (_skillCache == null)
            {
                // ★変更点 2: _skillCache を初期化
                _skillCache = new List<Skill>();

                // ★変更点 3: 定義されたすべてのパスをループして読み込む
                foreach (var path in _skillResourcePaths)
                {
                    // Resources.LoadAll<T> で、指定パス配下の T 型アセットをすべて読み込む
                    var skillsInPath = Resources.LoadAll<Skill>(path);
                    
                    // 読み込んだアセットをキャッシュリストに追加
                    _skillCache.AddRange(skillsInPath);
                }

                // (任意) もし複数のフォルダに同じアセットが重複して置かれる可能性がある場合
                // Distinct() で重複を除外します
                _skillCache = _skillCache.Distinct().ToList();
            }

            foreach (var skill in _skillCache)
                Skills[skill.ToString()] = skill;

            return _skillCache;
        }

        public IEnumerable<Skill> GetSkillsByLevel(DeckLevel level)
        {
            // こちらのメソッドは変更不要です
            // GetAllSkills() がすべてのスキルを返すため、
            // このLINQは正しく動作します
            return GetAllSkills().Where(skill => skill.Level == level);
        }
    }
}
