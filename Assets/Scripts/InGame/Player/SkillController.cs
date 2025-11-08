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
        [SerializeField] private Skill[] skill = new Skill[4];
        [SerializeField] private int index = 0;
        private GameObject _player;

        public void SetPlayer(GameObject p) => _player = p;

        // PlayerMaker から呼ばれる想定
        public void SetSelectedSkills(SkillSet set)
        {
            if (set == null)
            {
                Debug.LogWarning("SkillController: SkillSet is null");
                return;
            }
            skill[1] = set.Level1Skill;
            skill[2] = set.Level2Skill;
            skill[3] = set.Level3Skill;
            Debug.Log("SkillController: Selected skills were set.");
        }

        public void BreakCrystal()
        {
            index++;
            Debug.Log($"{index}番目を開放");
            if (index < 4) skill[index]?.Initialize(_player);
            else JudgeVictory.Instance.photonView.RPC(nameof(JudgeVictory.AddClearIncident), RpcTarget.AllViaServer);
        }
    }
}