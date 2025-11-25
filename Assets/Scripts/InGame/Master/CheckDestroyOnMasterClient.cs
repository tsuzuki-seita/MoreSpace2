using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

namespace MoreSpace.InGame.Master
{
    public class CheckDestroyOnMasterClient : SingletonMonoBehaviourPunCallbacks<CheckDestroyOnMasterClient>
    {
        // タイムスタンプ保存用
        private Dictionary<int, int?> _masterDeadTimestamps = new Dictionary<int, int?>();
        private Dictionary<int, int?> _userDeadTimestamps = new Dictionary<int, int?>();
        
        // 判定待ちの状態を管理するセット
        private HashSet<int> _pendingCheckIDs = new HashSet<int>();
        // 判定済みID（二重送信防止）
        private HashSet<int> _processedIDs = new HashSet<int>();

        // 他のプレイヤーのパケット遅延を考慮する待機時間(秒)
        // Ping値に応じて調整が必要だが、0.1~0.2秒あれば概ねカバー可能
        [SerializeField] private float _judgeWaitBuffer = 0.2f;

        [PunRPC]
        public void RPC_ReportDeath(int deadViewID, PhotonMessageInfo info)
        {
            if (!PhotonNetwork.IsMasterClient) return; 
            
            // 既に判定が終わっているIDなら無視
            if (_processedIDs.Contains(deadViewID)) return;

            int timestamp = info.SentServerTimestamp;

            if (info.Sender.IsMasterClient)
            {
                if (_masterDeadTimestamps.ContainsKey(deadViewID)) return;
                _masterDeadTimestamps[deadViewID] = timestamp;
            }
            else
            {
                if (_userDeadTimestamps.ContainsKey(deadViewID)) return;
                _userDeadTimestamps[deadViewID] = timestamp;
            }

            Debug.Log($"[Report] ID:{deadViewID} Time:{timestamp} From:{(info.Sender.IsMasterClient ? "Master" : "User")}");

            // まだ判定待ちループが回っていなければ開始
            if (!_pendingCheckIDs.Contains(deadViewID))
            {
                _pendingCheckIDs.Add(deadViewID);
                StartCoroutine(WaitAndCheckHealth(deadViewID));
            }
        }

        // IDごとに個別のコルーチンで待機する
        IEnumerator WaitAndCheckHealth(int checkID)
        {
            // ラグを考慮して少し待つ
            yield return new WaitForSeconds(_judgeWaitBuffer);

            _pendingCheckIDs.Remove(checkID);
            
            // 待機中に判定済みになっていれば終了
            if (_processedIDs.Contains(checkID)) yield break;
            
            CheckHealth(checkID);
        }

        private void CheckHealth(int checkID)
        {
            _masterDeadTimestamps.TryGetValue(checkID, out int? masterTime);
            _userDeadTimestamps.TryGetValue(checkID, out int? userTime);

            bool isMasterReport = masterTime != null;
            bool isUserReport = userTime != null;
            
            if (isMasterReport || isUserReport)
            {
                // 判定完了としてマーク
                _processedIDs.Add(checkID);

                bool isMasterBreak = false;
                bool isDraw = false;

                if (isMasterReport && isUserReport)
                {
                    // タイムスタンプ比較 (値が小さい＝時間が早い＝勝ち)
                    if (masterTime.Value == userTime.Value)
                    {
                        isDraw = true;
                    }
                    else if (masterTime.Value < userTime.Value)
                    {
                        isMasterBreak = true;
                    }
                    else
                    {
                        isMasterBreak = false; // Userの方が早い
                    }
                }
                else if (isMasterReport)
                {
                    isMasterBreak = true;
                }
                else if (isUserReport)
                {
                    isMasterBreak = false;
                }
                
                // データクリア（メモリリーク防止のため適当なタイミングで消す、あるいはScene遷移でリセット）
                // ここでRemoveすると後から遅れてきたパケットで再判定してしまう恐れがあるため、
                // _processedIDs でガードしつつデータは残すか、十分時間が経ってから消すのが安全。
                
                photonView.RPC(nameof(RPC_Dead), RpcTarget.All, checkID, isMasterBreak, isDraw);
            }
        }

        [PunRPC]
        public void RPC_Dead(int brokeObjectID, bool isBrokeMasterClient, bool isDraw)
        {
            // マスタークライアントのみ、破壊判定済みリストに入れておく（同期ズレ防止）
            if (PhotonNetwork.IsMasterClient) _processedIDs.Add(brokeObjectID);

            Photon.Realtime.Player winPlayer = PhotonNetwork.MasterClient;
            if (!isDraw && !isBrokeMasterClient)
            {
                // Masterじゃないプレイヤーを探す（2人対戦前提）
                winPlayer = PhotonNetwork.PlayerList.FirstOrDefault(p => !p.IsMasterClient);
            }
            
            // プレイヤーが見つからない場合（退出済みなど）のガード
            if (winPlayer == null) return;

            Debug.Log($"Winner: {winPlayer.ActorNumber} (Draw: {isDraw})");
            IDamageable breakObject = DamageableHolder.GetInstance(brokeObjectID);
            
            // 既にDestroyされている場合のNullチェック
            if (breakObject as UnityEngine.Object != null)
            {
                 breakObject.Die(winPlayer);
            }
        }
        
        // シーン遷移時などに呼び出してリセットする
        public void ResetData()
        {
            _masterDeadTimestamps.Clear();
            _userDeadTimestamps.Clear();
            _pendingCheckIDs.Clear();
            _processedIDs.Clear();
        }
    }
}