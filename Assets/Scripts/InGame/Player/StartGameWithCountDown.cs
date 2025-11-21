using System;
using System.Collections;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace MoreSpace.InGame.Player
{
    public class StartGameWithCountDown : MonoBehaviourPunCallbacks
    {
        public static bool isStartGame { get; private set; } = false;
        
        [SerializeField] private Text _countdownText;

        // 遷移直後の通信負荷や遅延を考慮したバッファ
        [SerializeField] private double _startDelayBuffer = 1.0f;
        [SerializeField] private double _countDownTime = 3.0f;

        private double _targetStartTime = -1;

        private const string KEY_IS_LOADED = "IsLoaded";

        private void Start()
        {
            isStartGame = false;
            WriteCountDownText(-1);
        }

        public void OnEndPrepare()
        {
            var props = new Hashtable { { KEY_IS_LOADED, true } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
            if (!PhotonNetwork.IsMasterClient) return;

            if (changedProps.ContainsKey(KEY_IS_LOADED))
            {
                CheckAllPlayersLoaded();
            }
        }

        private void CheckAllPlayersLoaded()
        {
            foreach (var p in PhotonNetwork.PlayerList)
            {
                if (!p.CustomProperties.ContainsKey(KEY_IS_LOADED) || !(bool)p.CustomProperties[KEY_IS_LOADED])
                {
                    return;
                }
            }

            // 全員準備完了時に未来の時刻を設定 (現在時刻 + カウントダウン秒数 + バッファ)
            double startTime = PhotonNetwork.Time + _countDownTime + _startDelayBuffer;
            photonView.RPC(nameof(RpcSetStartTime), RpcTarget.AllBuffered, startTime);
        }

        [PunRPC]
        private void RpcSetStartTime(double timestamp)
        {
            _targetStartTime = timestamp;
            StartCoroutine(CountDown());
        }

        IEnumerator CountDown()
        {
            // サーバー時刻との差分を計算
            double timeRemaining = double.PositiveInfinity;

            while(timeRemaining > 0)
            {
                WriteCountDownText(timeRemaining);
                yield return null;
                timeRemaining = _targetStartTime - PhotonNetwork.Time;
            }
            
            _countdownText.text = "START!";
            StartGame();

            yield return new WaitForSeconds(1);
            yield return _countdownText.DOFade(0, 0.25f).WaitForCompletion();
            _countdownText.gameObject.SetActive(false);
        }

        void WriteCountDownText(double timeRemaining)
        {
            if (timeRemaining is > 0 and < 3)
                _countdownText.text = Mathf.CeilToInt(Mathf.Clamp((float)timeRemaining,0,(float)_countDownTime)).ToString();
            else
                _countdownText.text = "Waiting";
        }

        private void StartGame()
        {
            isStartGame = true;

            Debug.Log("Game Started Sync at: " + PhotonNetwork.Time);
        }
    }
}
