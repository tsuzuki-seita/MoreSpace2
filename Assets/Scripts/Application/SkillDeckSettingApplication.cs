using UnityEngine;
using Cysharp.Threading.Tasks;
using R3;
using MoreSpace.Domain;
using System;
using System.Collections.Generic;
using VContainer;
using System.Linq;

namespace MoreSpace.Application
{
    public interface ISkillRepository
    {
        // プロジェクト内の全スキルSOを取得する
        IEnumerable<Skill> GetAllSkills();

        // 特定レベルのスキルSOのみを取得する
        IEnumerable<Skill> GetSkillsByLevel(DeckLevel level);
    }
    
    public sealed class SkillSelectionService
    {
        private readonly NavigationService _nav;
        private readonly ISkillRepository _skillRepo;

        // DI (VContainerを想定)
        [Inject]
        public SkillSelectionService(NavigationService nav, ISkillRepository repo)
        {
            _nav = nav;
            _skillRepo = repo;
        }

        // 該当レベルのスキルリストを取得する
        public List<Skill> GetSkillsForLevel(DeckLevel level)
        {
            return _skillRepo.GetSkillsByLevel(level).ToList();
        }

        // 決定ボタンが押された時の処理
        public void ConfirmSelection(Skill s1, Skill s2, Skill s3)
        {
            // 1. スキルセットを作成
            var skillSet = new SkillSet(s1, s2, s3);
            
            // 2. 遷移用の引数クラスを作成
            var args = new StartIngameArgs(skillSet);

            // 3. 既存のNavigationServiceを使ってシーン遷移
            // (IngameState.Ingame は仮。適切なシーン名に変更してください)
            _nav.ChangeScene(InGameState.Ingame, args);
        }

        // 戻るボタンが押された時の処理
        public void GoBackToTitle()
        {
            // (IngameState.Title は仮。適切なシーン名に変更してください)
            _nav.ChangeScene(InGameState.Title);
        }
    }
}