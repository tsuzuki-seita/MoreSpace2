using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace MoreSpace.InGame.Master
{
    public class CheckDestroyOnMasterClient : SingletonMonoBehaviourPunCallbacks<CheckDestroyOnMasterClient>
    {
        private Dictionary<int, int?> _masterDeadTimestamps = new Dictionary<int, int?>();
        private Dictionary<int, int?> _userDeadTimestamps = new Dictionary<int, int?>();
        
        private List<int> _checkIDList = new List<int>();
        private bool _isQueueCheck = false;

        [PunRPC]
        public void RPC_ReportDeath(int deadViewID, PhotonMessageInfo info)
        {
            Debug.Log($"破壊確認が送信されました");
            if (!PhotonNetwork.IsMasterClient) return; 

            Debug.Log($"マスターが受信しました");
            int timestamp = info.SentServerTimestamp;
            if (info.Sender.Equals(PhotonNetwork.MasterClient))
            {
                if (_masterDeadTimestamps.ContainsKey(deadViewID)) return;
                Debug.Log($"Master");
                _masterDeadTimestamps[deadViewID] = timestamp;
            }
            else
            {
                if (_userDeadTimestamps.ContainsKey(deadViewID)) return;
                Debug.Log($"User");
                _userDeadTimestamps[deadViewID] = timestamp;
            }

            Debug.Log($"[GameManager] {deadViewID} の死亡報告を受理。時刻: {timestamp}");

            _checkIDList.Add(deadViewID);
            if (!_isQueueCheck)
            {
                StartCoroutine(RunCheckHealth());
            }
        }

        IEnumerator RunCheckHealth()
        {
            _isQueueCheck = true;
            yield return null;
            foreach(var target in _checkIDList)
                CheckHealth(target);
            _checkIDList.Clear();
            _isQueueCheck = false;
        }

        private void CheckHealth(int checkID)
        {
            _masterDeadTimestamps.TryGetValue(checkID, out int? masterTime);
            _userDeadTimestamps.TryGetValue(checkID, out int? userTime);

            bool isMasterDead = masterTime != null;
            bool isUserDead = userTime != null;
            
            Debug.Log($"{isMasterDead}/{isUserDead}");
            if (isMasterDead || isUserDead)
            {
                photonView.RPC(nameof(RPC_Dead), RpcTarget.All,checkID, isMasterDead, isMasterDead && isUserDead);
            }
        }

        [PunRPC]
        public void RPC_Dead(int brokeObjectID,bool isBrokeMasterClient, bool isDraw)
        {
            IDamageable breakObject = DamageableHolder.GetInstance(brokeObjectID);
            breakObject?.Die(); 

            if (isDraw)
            {
                Debug.Log("同一フレームで破壊処理が行われたのはここ");
            }
            else
            {
                if(PhotonNetwork.IsMasterClient == isBrokeMasterClient)
                    Debug.Log("破壊したプレイヤーはこれ");
            }
        }
    }
}