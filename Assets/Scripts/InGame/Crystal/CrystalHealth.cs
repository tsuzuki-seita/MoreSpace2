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
        [SerializeField] private MeshRenderer crystalMesh;
        [SerializeField] private MeshCollider crystalCollider;
        [Header("惑星の色替え演出用")]
        [SerializeField] private MeshRenderer planetMesh;
        [SerializeField] private Color[] changedColor = new Color[2];
        private Material _instance;
        private static readonly int Degree = Shader.PropertyToID("_Degree");

        protected override void OnInitialize()
        {
            DetachParticles();
            _instance = crystalMesh.material;
            OnDamage += (hp,maxHp) =>
            {
                _instance.SetFloat(Degree,(float)hp/maxHp);
                if(hp > 0)
                {
                    damageParticle.Emit(1);
                    SoundManager.Instance.PlaySE(SoundManager.SEData.SETYPE.CrystalDamage);
                }
                else 
                    destroyParticle.Emit(1);
            };
            OnHpZero += () =>
            {
                crystalMesh.enabled = false;
                crystalCollider.enabled = false;
            };
        }

        public override void Die(Photon.Realtime.Player doPlayer)
        {
            SoundManager.Instance.PlaySE(SoundManager.SEData.SETYPE.CrystalBreak);
            if(doPlayer.Equals(PhotonNetwork.LocalPlayer)) SkillController.Instance.BreakCrystal();
            ChangeColor(doPlayer);
            Destroy(this.gameObject);
        }

        void ChangeColor(Photon.Realtime.Player doPlayer)
        {
            var playerIndex = doPlayer.IsMasterClient ? 0 : 1;
            planetMesh.material.mainTexture = null;
            planetMesh.material.color = changedColor[playerIndex];
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
