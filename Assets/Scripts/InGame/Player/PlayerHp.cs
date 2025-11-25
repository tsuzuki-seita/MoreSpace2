using UnityEngine;
using MoreSpace.InGame.Master;
using Photon.Pun;
using R3;

namespace MoreSpace.InGame.Player
{
    public class PlayerHp : HealthBase
    {
        [SerializeField] private PlayerBuffs playerBuffs;
        // 外部（Unbreakableスキル）から操作するためのフラグ
        public bool IsUnbreakable = false;

        // 親クラスの判定をオーバーライド
        // IsUnbreakableが true なら、CanTakeDamage は false (ダメージ受けない) になる
        protected override bool CanTakeDamage => !IsUnbreakable;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            OnDamage += (hp, maxHp) =>
            {
                if (hp >= 0)
                {
                    SoundManager.Instance.PlaySE(SoundManager.SEData.SETYPE.PlayerDamage);
                }
            };
            
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

