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
        [SerializeField] private ParticleSystem destroyParticle;
        [SerializeField] private MeshRenderer mesh;
        private Material _instance;
        private static readonly int Degree = Shader.PropertyToID("_Degree");

        protected override void OnInitialize()
        {
            DetachParticles();
            _instance = mesh.material;
            OnDamage += (hp,maxHp) =>
            {
                _instance.SetFloat(Degree,(float)hp/maxHp);
                if(hp > 0)
                    damageParticle.Emit(1);
                else 
                    destroyParticle.Emit(1);
            };
            OnHpZero += () => Destroy(this.gameObject);
        }

        public override void Die(Photon.Realtime.Player doPlayer)
        {
            SoundManager.Instance.PlaySE(SoundManager.SEData.SETYPE.CrystalBreak);
            if(doPlayer.Equals(PhotonNetwork.LocalPlayer)) SkillController.Instance.BreakCrystal();
        }

        void DetachParticles()
        {
            var scaleValue = damageParticle.transform.localScale.x;
            damageParticle.transform.SetParent(null);
            destroyParticle.transform.SetParent(null);

            damageParticle.transform.localScale = Vector3.one * scaleValue;
            destroyParticle.transform.localScale = Vector3.one * scaleValue;
        }
    }
}
