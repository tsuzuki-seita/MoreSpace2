using System;
using MoreSpace.InGame.Master;
using Photon.Pun;
using UnityEngine;

namespace MoreSpace.InGame
{
    public class HealthBase : MonoBehaviourPunCallbacks, IDamageable
    {
        // [SerializeField] int hp = 100;
        public int hp = 100;
        private int maxHp;
        public Action<int, int> OnDamage;
        protected Action OnHpZero;

        /// <summary>
        /// ダメージを受けることが可能かどうかを判定するプロパティ。
        /// 派生クラスで条件（無敵モードなど）を追加可能にします。
        /// </summary>
        protected virtual bool CanTakeDamage => true;

        private void Start()
        {
            DamageableHolder.Holders.Add(photonView.ViewID, this);
            maxHp = hp;
            OnInitialize();
        }
        
        protected virtual void OnInitialize(){}

        public void Damage(int damage)
        {
            photonView.RPC(nameof(DamageOnRPC), RpcTarget.All, damage);
        }

        [PunRPC]
        protected void DamageOnRPC(int rawDamage, PhotonMessageInfo info)
        {
            // --- 追加箇所: ダメージ判定チェック ---
            if (!CanTakeDamage)
            {
                // 無敵状態などでダメージが無効化された場合
                Debug.Log($"{gameObject.name}はダメージを無効化しました。");
                return;
            }
            // ----------------------------------

            hp -= damage;
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