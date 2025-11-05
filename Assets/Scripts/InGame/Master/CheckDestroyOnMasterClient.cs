using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace MoreSpace.InGame.Master
{
    public class CheckDestroyOnMasterClient : MonoBehaviourPunCallbacks
    {
        private Dictionary<int, int> _masterDeadTimestamps = new Dictionary<int, int>();
        private Dictionary<int, int> _userDeadTimestamps = new Dictionary<int, int>();

        [PunRPC]
        public void RPC_ReportDeath(int deadViewID, PhotonMessageInfo info)
        {
            if (!PhotonNetwork.IsMasterClient) return; 
            if (_masterDeadTimestamps.ContainsKey(deadViewID)) return;

            int timestamp = info.SentServerTimestamp;
            if(info.Sender.Equals(PhotonNetwork.LocalPlayer))
                _masterDeadTimestamps[deadViewID] = timestamp;
            else
                _userDeadTimestamps[deadViewID] = timestamp;
            Debug.Log($"[GameManager] {deadViewID} の死亡報告を受理。時刻: {timestamp}");

            CheckPlayerWinCondition(deadViewID);
        }

        private void CheckPlayerWinCondition(int checkID)
        {
            _masterDeadTimestamps.TryGetValue(checkID, out int masterTime);
            _userDeadTimestamps.TryGetValue(checkID, out int userTime);

            bool masterDead = masterTime > 0;
            bool userDead = userTime > 0;

            // どちらかのプレイヤーが死んだ時点でゲーム終了
            if (masterDead || userDead)
            {
                photonView.RPC(nameof(RPC_Dead), RpcTarget.All, masterDead, masterDead && userDead);
            }
        }

        [PunRPC]
        public void RPC_Dead(int brokeObjectID,bool brokeMasterClient, bool isDraw)
        {
            IDamageable loserHealth = DamageableHolder.GetInstance(brokeObjectID);
            loserHealth?.Die(); 

            if (isDraw)
            {
                
            }
            else
            {
                
            }
        }
    }
}