using UnityEngine;
using MoreSpace.InGame.Master;
using Photon.Pun;
using R3;

namespace MoreSpace.InGame.Player
{
    public class PlayerHp : HealthBase
    {
        [SerializeField] private PlayerBuffs playerBuffs;
        protected override void OnInitialize()
        {
            base.OnInitialize();
            
            if (playerBuffs != null)
            {
                playerBuffs.Defense
                    .Subscribe(defenseBonus => 
                    {
                        this._defenseBonus = defenseBonus; 
                        Debug.Log($"[{gameObject.name}] 防御力ボーナスが更新されました: {this._defenseBonus}");
                    })
                    .AddTo(this); 
            }
        }
        public override void Die(Photon.Realtime.Player doPlayer)
        {
            if(doPlayer.Equals(PhotonNetwork.LocalPlayer)) JudgeVictory.Instance.photonView.RPC(nameof(JudgeVictory.AddClearIncident),RpcTarget.AllViaServer);
        }
    }
}

