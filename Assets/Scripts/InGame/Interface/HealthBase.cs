using System;
using MoreSpace.InGame.Master;
using Photon.Pun;
using UnityEngine;

namespace MoreSpace.InGame
{
    public class HealthBase : MonoBehaviourPunCallbacks, IDamageable
    {
        [SerializeField] int hp = 100;
        protected Action OnDamage;
        private CheckDestroyOnMasterClient master;
        private void Start()
        {
            master = FindAnyObjectByType<CheckDestroyOnMasterClient>();
            DamageableHolder.Holders.Add(photonView.ViewID,this);
            OnInitialize();
        }
        
        protected virtual void OnInitialize(){}

        public void Damage(int damage)
        {
            photonView.RPC(nameof(DamageOnRPC),RpcTarget.All,damage);
        }

        [PunRPC]
        protected void DamageOnRPC(int damage)
        {
            hp -= damage;
            OnDamage?.Invoke();
            Debug.Log($"{gameObject.name}が{damage}受けています, 残りHP: {hp}");
            if (hp <= 0)
            {
                master.photonView.RPC(nameof(master.RPC_ReportDeath),RpcTarget.All,this.photonView.ViewID);
            }
        }

        private void OnDestroy()
        {
            Debug.LogError("Destroy" + name);
        }

        public virtual void Die(){}
    }
}