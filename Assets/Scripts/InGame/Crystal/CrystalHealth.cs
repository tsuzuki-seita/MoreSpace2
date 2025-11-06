using System;
using MoreSpace.InGame.Master;
using Photon.Pun;
using UnityEngine;

namespace MoreSpace.InGame
{
    public class CrystalHealth : HealthBase
    {
        [SerializeField] private GameObject damageParticlePrefab;

        protected override void OnInitialize()
        {
            OnDamage += () => Instantiate(damageParticlePrefab, this.transform.position, Quaternion.identity);
        }

        public override void Die()
        {
            Destroy(this.gameObject);
        }
    }
}
