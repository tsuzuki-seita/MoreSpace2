using System;
using UnityEngine;

namespace MoreSpace.InGame
{
    public class CrystalHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private float hp = 50;
        [SerializeField] private GameObject damageParticlePrefab;

        public void Damage(int damage)
        {
            hp -= damage;
            Instantiate(damageParticlePrefab, this.transform.position, Quaternion.identity);
            if(hp < damage) Destroy(this.gameObject);
        }
    }
}
