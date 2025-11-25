using System.Collections;
using MoreSpace.Application;
using MoreSpace.Domain;
using MoreSpace.Presentation;
using Photon.Pun;
using UnityEngine;

namespace MoreSpace.InGame.Master
{
    public class JudgeVictory : SingletonMonoBehaviourPunCallbacks<JudgeVictory>
    {
        private int?[] isWin = new int?[2];
        private bool _isQueueCheck = false;
        private bool _isDisconnectUser = false;
        private bool _isFinishGame = false;
        
        [PunRPC]
        public void AddClearIncident(PhotonMessageInfo info)
        {
            Debug.Log($"破壊確認が送信されました");
            if(_isFinishGame) return;
            _isFinishGame = true;
            
            if (!PhotonNetwork.IsMasterClient) return; 

            Debug.Log($"マスターが受信しました");
            int timestamp = info.SentServerTimestamp;
            if (info.Sender.Equals(PhotonNetwork.MasterClient))
            {
                if (isWin[0] != null) return;
                Debug.Log($"Master");
                isWin[0] = timestamp;
            }
            else
            {
                if (isWin[1] != null) return;
                Debug.Log($"User");
                isWin[1] = timestamp;
            }

            Debug.Log($"[GameManager] Player{(info.Sender.Equals(PhotonNetwork.MasterClient) ? 1 : 2)} の勝利報告を受理。時刻: {timestamp}");

            if (!_isQueueCheck)
            {
                StartCoroutine(RunCheckWin());
            }
        }

        IEnumerator RunCheckWin()
        {
            _isQueueCheck = true;
            yield return null;
            Judge();
            _isQueueCheck = false;
        }

        void Judge()
        {
            bool isMasterReport = isWin[0] != null;
            bool isUserReport = isWin[1] != null;
            Debug.Log($"{isMasterReport}/{isUserReport}");
            if (isMasterReport || isUserReport)
            {
                bool isMasterWin = false;
                bool isDraw = false;

                // 1. 両方から報告があった場合 (タイムスタンプを比較)
                if (isMasterReport && isUserReport)
                {
                    // (int?)型なので .Value で値を取得
                    if (isWin[0].Value == isWin[1].Value)
                    {
                        isDraw = true;
                    }
                    else if (isWin[0].Value < isWin[1].Value)
                    {
                        isMasterWin = true;
                    }
                    else // isWin[0].Value > isWin[1].Value
                    {
                        isMasterWin = false;
                    }
                }
                // 2. マスターからのみ報告があった場合
                else if (isMasterReport)
                {
                    // ユーザーの勝ち
                    isMasterWin = true;
                }
                // 3. ユーザーからのみ報告があった場合
                else if (isUserReport)
                {
                    // マスターの勝ち
                    isMasterWin = false;
                }

                photonView.RPC(nameof(RPC_Win), RpcTarget.All, isMasterWin, isDraw);
            }
        }

        [PunRPC]
        void RPC_Win(bool isMasterWin, bool isDraw)
        {
            ResultPattern result = ResultPattern.Draw;
            if (!isDraw)
            {
                result = isMasterWin == PhotonNetwork.IsMasterClient ? ResultPattern.Win : ResultPattern.Lose;
            }

            StartCoroutine(WaitToFinishUser(result));
        }

        IEnumerator WaitToFinishUser(ResultPattern result)
        {
            if(PhotonNetwork.IsMasterClient)
                yield return new WaitUntil(() => _isDisconnectUser);
            PhotonNetwork.Disconnect();
            IngameSceneManager.Instance.ChangeScene(InGameState.Result, new ResultArgs(result));
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            Disconnect();   
        }

        public override void OnLeftRoom()
        {
            Disconnect();
        }

        void Disconnect()
        {
            if (_isFinishGame)
            {
                _isDisconnectUser = true;
                return;
            }
            _isFinishGame = true;
            IngameSceneManager.Instance.ChangeScene(InGameState.Result, new ResultArgs(ResultPattern.Disconnect));
            Debug.Log($"切断");
        }
    }

    public enum ResultPattern
    {
        Win,
        Lose,
        Draw,
        Disconnect,
        Finish
    }
    public sealed class ResultArgs : ITransientArgs
    {
        public ResultPattern Result;
        public ResultArgs(ResultPattern r)
        {
            Result = r;
        }
    }
}