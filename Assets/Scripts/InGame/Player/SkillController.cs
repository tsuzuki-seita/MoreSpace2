using System;
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
        [SerializeField] private PlayerObjectHolder playerHolder;
        [SerializeField] private Skill[] skill = new Skill[4];
        [SerializeField] private int index = 0;
        private List<(string, int)> _cacheAddSkill = new List<(string, int)>();

        private void Awake()
        {
            playerHolder.OnAddPlayer += SetPlayer;
        }

        private void SetPlayer(PhotonView p)
        {
            foreach (var cache in _cacheAddSkill)
                if(cache.Item2 == p.ViewID)
                    InitializeSkill(cache.Item1,cache.Item2);
        }

        public void SetSelectedSkills(SkillSet set)
        {
            Debug.Log(playerHolder.player[PhotonNetwork.LocalPlayer]);
            
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
            
            playerHolder.player[PhotonNetwork.LocalPlayer].GetComponent<SkillViewer>().VisualizeSkills(skill);
            photonView.RPC(nameof(InitializeSkill),RpcTarget.All,skill[index].ToString(), playerHolder.player[PhotonNetwork.LocalPlayer].ViewID);
        }

        public void BreakCrystal()
        {
            index++;
            Debug.Log($"{index}番目を開放");
            if (index < 4) photonView.RPC(nameof(InitializeSkill),RpcTarget.All,skill[index].ToString(), playerHolder.player[PhotonNetwork.LocalPlayer].ViewID);
            else JudgeVictory.Instance.photonView.RPC(nameof(JudgeVictory.AddClearIncident), RpcTarget.AllViaServer);
        }

        [PunRPC]
        public void InitializeSkill(string target, int id)
        {
            Debug.Log($"InitializeSkill:{id}/{target}");
            var targetPlayer = playerHolder.player.FirstOrDefault(p => p.Value.ViewID == id).Value;
            if (targetPlayer == null)
                _cacheAddSkill.Add((target,id));
            else
                ResourceSkillRepository.Skills[target].Initialize(targetPlayer.gameObject);
        }
        
        private void OnDestroy()
        {
            playerHolder.OnAddPlayer -= SetPlayer;
        }
    }
}