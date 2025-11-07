using System;
using MoreSpace.InGame.Master;
using Photon.Pun;
using UnityEngine;

namespace MoreSpace.InGame
{
    public class HealthBase : MonoBehaviourPunCallbacks, IDamageable
    {
        [SerializeField] int hp = 100;
        public Action<int> OnDamage;
        private void Start()
        {
            DamageableHolder.Holders.Add(photonView.ViewID,this);
            OnInitialize();
        }
        
        protected virtual void OnInitialize(){}

        public void Damage(int damage)
        {
            photonView.RPC(nameof(DamageOnRPC),RpcTarget.All,damage);
        }

        [PunRPC]
        protected void DamageOnRPC(int damage, PhotonMessageInfo info)
        {
            hp -= damage;
            OnDamage?.Invoke(hp);
            Debug.Log($"{gameObject.name}が{damage}受けています, 残りHP: {hp}");
            if (hp <= 0 && info.Sender.Equals(PhotonNetwork.LocalPlayer))
            {
                Debug.Log("破壊処理を送信します");
                CheckDestroyOnMasterClient.Instance.photonView.RPC(nameof(CheckDestroyOnMasterClient.RPC_ReportDeath),RpcTarget.AllViaServer,this.photonView.ViewID);
            }
        }

        private void OnDestroy()
        {
            Debug.Log("Destroy" + name);
        }

        public virtual void Die(Photon.Realtime.Player doPlayer){}
    }
}