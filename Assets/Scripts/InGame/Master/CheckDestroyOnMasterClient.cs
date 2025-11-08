using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            if (info.Sender.IsMasterClient)
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

            if(!_checkIDList.Contains(deadViewID)) _checkIDList.Add(deadViewID);
            if (!_isQueueCheck)
                StartCoroutine(RunCheckHealth());
        }

        IEnumerator RunCheckHealth()
        {
            _isQueueCheck = true;
            yield return null;
            while (_checkIDList.Count > 0)
            {
                // 1. 処理するリストをコピー (リストのロック時間を最小限にする)
                var listToProcess = new List<int>(_checkIDList);
                _checkIDList.Clear();
                
                foreach(var target in listToProcess)
                    CheckHealth(target);
            }
            _isQueueCheck = false;
        }

        private void CheckHealth(int checkID)
        {
            _masterDeadTimestamps.TryGetValue(checkID, out int? masterTime);
            _userDeadTimestamps.TryGetValue(checkID, out int? userTime);

            bool isMasterReport = masterTime != null;
            bool isUserReport = userTime != null;
            
            Debug.Log($"{isMasterReport}/{isUserReport}");
            if (isMasterReport || isUserReport)
            {
                bool isMasterBreak = false;
                bool isDraw = false;

                // 1. 両方から報告があった場合 (タイムスタンプを比較)
                if (isMasterReport && isUserReport)
                {
                    // (int?)型なので .Value で値を取得
                    if (masterTime.Value == userTime.Value)
                    {
                        isDraw = true;
                    }
                    else if (masterTime.Value < userTime.Value)
                    {
                        isMasterBreak = true;
                    }
                    else // masterTime.Value > userTime.Value
                    {
                        isMasterBreak = false;
                    }
                }
                // 2. マスターからのみ報告があった場合 (マスターが死んだ)
                else if (isMasterReport)
                {
                    // ユーザーの勝ち
                    isMasterBreak = true;
                }
                // 3. ユーザーからのみ報告があった場合 (ユーザーが死んだ)
                else if (isUserReport)
                {
                    // マスターの勝ち
                    isMasterBreak = false;
                }
                photonView.RPC(nameof(RPC_Dead), RpcTarget.All,checkID, isMasterBreak, isDraw);
            }
        }

        [PunRPC]
        public void RPC_Dead(int brokeObjectID,bool isBrokeMasterClient, bool isDraw)
        {
            //ドローならマスター優先
            Photon.Realtime.Player winPlayer = PhotonNetwork.MasterClient;
            if (!isDraw && !isBrokeMasterClient)
            {
                winPlayer = PhotonNetwork.PlayerList.First(p => !p.IsMasterClient);
            }
            
            Debug.Log($"破壊したのは{winPlayer.ActorNumber} これと等しい?{winPlayer.Equals(PhotonNetwork.LocalPlayer)}");
            IDamageable breakObject = DamageableHolder.GetInstance(brokeObjectID);
            breakObject?.Die(winPlayer);
        }
    }
}