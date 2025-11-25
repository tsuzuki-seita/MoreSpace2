using System;
using System.Collections.Generic;
using MoreSpace.InGame.Master;
using MoreSpace.InGame.Player;
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
        public Action OnHpZero;
        protected float _defenseBonus = 0f;
        private static Dictionary<Photon.Realtime.Player, VisualizeWireOnAttackCrystal> _visualizeDictionary = new();

        protected virtual bool CanTakeDamage => true;

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
            // --- 追加箇所: ダメージ判定チェック ---
            if (!CanTakeDamage)
            {
                // 無敵状態などでダメージが無効化された場合
                Debug.Log($"{gameObject.name}はダメージを無効化しました。");
                return;
            }

            int finalDamage = rawDamage - Mathf.RoundToInt(_defenseBonus);

            hp -= finalDamage;
            OnDamage?.Invoke(hp, maxHp);
            Debug.Log($"{gameObject.name}が{rawDamage}をうけて{_defenseBonus}で守り最終{finalDamage}受けています, 残りHP: {hp}");
            if (hp <= 0)
            {
                OnHpZero?.Invoke();
                if (info.Sender.IsLocal)
                {
                    Debug.Log($"私が壊したので破壊処理を送信します");
                    CheckDestroyOnMasterClient.Instance.photonView.RPC(
                        nameof(CheckDestroyOnMasterClient.RPC_ReportDeath), RpcTarget.AllViaServer,
                        this.photonView.ViewID);
                }
            }
            
            //ワイヤーの表示
            if (!_visualizeDictionary.ContainsKey(info.Sender))
            {
                _visualizeDictionary.Add(info.Sender,PlayerObjectHolder.Instance.player[info.Sender].GetComponent<VisualizeWireOnAttackCrystal>());
            }
            _visualizeDictionary[info.Sender].StartVisualize();
        }

        public virtual void Die(Photon.Realtime.Player doPlayer){}
    }
}