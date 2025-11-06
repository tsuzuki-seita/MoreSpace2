using System;
using MoreSpace.Domain;
using MoreSpace.InGame.Master;
using MoreSpace.Presentation;
using Photon.Pun;
using UnityEngine;

namespace MoreSpace.InGame.Player
{
    public class SkillController : SingletonMonoBehaviourPunCallbacks<SkillController>
    {
        [SerializeField] private Skill[] skill;
        [SerializeField] private int index = 0;
        private GameObject _player;
        public void SetPlayer(GameObject p)
        {
            _player = p;
        }

        private void Start()
        {
            InitializeSkillLists();
        }

        void InitializeSkillLists()
        {
            if (IngameSceneManager.Instance != null && IngameSceneManager.Instance.TryConsume<StartIngameArgs>(out var args))
            {
                var list = args.SelectedSkills;
                skill = new Skill[] {null, list.Level1Skill, list.Level2Skill, list.Level3Skill};
            }
            else
            {
                // スキルデータが渡されなかった場合 (デバッグ実行など)
                Debug.LogWarning("スキル選択データ (StartIngameArgs) が見つかりませんでした。");
            }
        }
        
        public void BreakCrystal()
        {
            index++;
            Debug.Log($"{index}番目を開放");
            if(index < 4) skill[index]?.Initialize(_player);
            else JudgeVictory.Instance.photonView.RPC(nameof(JudgeVictory.AddClearIncident),RpcTarget.AllViaServer);
        }
    }
}