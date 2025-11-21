using System.Collections.Generic;
using System.Linq;
using MoreSpace.Domain;
using MoreSpace.Infrastructure;
using MoreSpace.InGame.Master;
using NUnit.Framework;
using Photon.Pun;
using UnityEngine;

namespace MoreSpace.InGame.Player
{
    public class SkillController : SingletonMonoBehaviourPunCallbacks<SkillController>
    {
        [SerializeField] private Skill[] skill = new Skill[4];
        [SerializeField] private int index = 0;
        private List<PhotonView> _player = new List<PhotonView>();
        private List<(string, int)> _cacheAddSkill = new List<(string, int)>();
        
        public void SetPlayer(PhotonView p)
        {
            _player.Add(p);
            foreach (var cache in _cacheAddSkill)
                if(cache.Item2 == p.ViewID)
                    InitializeSkill(cache.Item1,cache.Item2);
        }

        public void SetSelectedSkills(SkillSet set)
        {
            Debug.Log(_player.First(p => p.IsMine));
            
            if (set != null)
            {
                skill[1] = set.Level1Skill;
                skill[2] = set.Level2Skill;
                skill[3] = set.Level3Skill;
            }
            else
            {
                Debug.LogWarning("SkillController: SkillSet is null. UseDefaultSkill");
            }
            Debug.Log("SkillController: Selected skills were set.");
            
            _player.First(p => p.IsMine).gameObject.GetComponent<SkillViewer>().VisualizeSkills(skill);
            photonView.RPC(nameof(InitializeSkill),RpcTarget.All,skill[index].ToString(), _player.First(p => p.IsMine).ViewID);
        }

        public void BreakCrystal()
        {
            index++;
            Debug.Log($"{index}番目を開放");
            if (index < 4) photonView.RPC(nameof(InitializeSkill),RpcTarget.All,skill[index].ToString(), _player.First(p => p.IsMine).ViewID);
            else JudgeVictory.Instance.photonView.RPC(nameof(JudgeVictory.AddClearIncident), RpcTarget.AllViaServer);
        }

        [PunRPC]
        public void InitializeSkill(string target, int id)
        {
            Debug.Log($"InitializeSkill:{id}/{target}");
            var targetPlayer = _player.FirstOrDefault(p => p.ViewID == id);
            if (targetPlayer == null)
                _cacheAddSkill.Add((target,id));
            else
                ResourceSkillRepository.Skills[target].Initialize(targetPlayer.gameObject);
        }
    }
}