using UnityEngine;
using MoreSpace.InGame.Master;
using Photon.Pun;

namespace MoreSpace.InGame.Player
{
    public class PlayerHp : HealthBase
    {
        // 外部（Unbreakableスキル）から操作するためのフラグ
        public bool IsUnbreakable = false;

        // 親クラスの判定をオーバーライド
        // IsUnbreakableが true なら、CanTakeDamage は false (ダメージ受けない) になる
        protected override bool CanTakeDamage => !IsUnbreakable;

        public override void Die(Photon.Realtime.Player doPlayer)
        {
            if(doPlayer.Equals(PhotonNetwork.LocalPlayer)) 
                JudgeVictory.Instance.photonView.RPC(nameof(JudgeVictory.AddClearIncident), RpcTarget.AllViaServer);
        }
    }
}