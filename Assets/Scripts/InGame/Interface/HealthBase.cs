using System;
using MoreSpace.InGame.Master;
using Photon.Pun;
using UnityEngine;

namespace MoreSpace.InGame
{
    public class HealthBase : MonoBehaviourPunCallbacks, IDamageable
    {
        [SerializeField] int hp = 100;
        private int maxHp;
        public Action<int, int> OnDamage;
        protected Action OnHpZero;
        protected float _defenseBonus = 0f;
        private void Start()
        {
            DamageableHolder.Holders.Add(photonView.ViewID,this);
            maxHp = hp;
            OnInitialize();
        }
        
        protected virtual void OnInitialize(){}

        public void Damage(int damage)
        {
            photonView.RPC(nameof(DamageOnRPC),RpcTarget.All,damage);
        }

        [PunRPC]
        protected void DamageOnRPC(int rawDamage, PhotonMessageInfo info)
        {
            int finalDamage = rawDamage - Mathf.RoundToInt(_defenseBonus);

            hp -= finalDamage;
            OnDamage?.Invoke(hp, maxHp);
            Debug.Log($"{gameObject.name}が{rawDamage}をうけて{_defenseBonus}で守り最終{finalDamage}受けています, 残りHP: {hp}");
            if (hp <= 0)
            {
                OnHpZero?.Invoke();
                if (info.Sender.Equals(PhotonNetwork.LocalPlayer))
                    CheckDestroyOnMasterClient.Instance.photonView.RPC(
                        nameof(CheckDestroyOnMasterClient.RPC_ReportDeath), RpcTarget.AllViaServer,
                        this.photonView.ViewID);
            }
        }

        public virtual void Die(Photon.Realtime.Player doPlayer){}
    }
}