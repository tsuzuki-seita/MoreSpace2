using System;
using MoreSpace.InGame.Master;
using MoreSpace.InGame.Player;
using Photon.Pun;
using UnityEngine;

namespace MoreSpace.InGame
{
    public class CrystalHealth : HealthBase
    {
        [SerializeField] private ParticleSystem damageParticle;

        protected override void OnInitialize()
        {
            OnDamage += (hp,maxHp) => damageParticle.Emit(1);
        }

        public override void Die(Photon.Realtime.Player doPlayer)
        {
            if(doPlayer.Equals(PhotonNetwork.LocalPlayer)) SkillController.Instance.BreakCrystal();
            Destroy(this.gameObject);
        }
    }
}
