using UnityEngine;
using MoreSpace.InGame.Master;
using Photon.Pun;

namespace MoreSpace.InGame.Player
{
    public class PlayerHp : HealthBase
    {
        public override void Die(Photon.Realtime.Player doPlayer)
        {
            if(doPlayer.Equals(PhotonNetwork.LocalPlayer)) JudgeVictory.Instance.photonView.RPC(nameof(JudgeVictory.AddClearIncident),RpcTarget.AllViaServer);
            Destroy(gameObject);
        }
    }
}

