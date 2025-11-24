using ObjectPool;
using UnityEngine;
using MoreSpace.InGame.Weapons.Bullets;
using R3;

namespace MoreSpace.InGame.Weapons
{
    public class SingleShot : Weapon
    {
        [SerializeField] public SingleBullet bullet;
        [SerializeField] public uint initCount = 10;
        [SerializeField] public float speed = 10;
        [SerializeField] public int damage = 10;
        [SerializeField] public float Distance = 10;

        private Pool<SingleBullet> pool;
        private int _finalDamage = 0;
        private int _finalObjectDamage = 0;
        
        protected override void InitializeBuffsAndSubscribe()
        {
            _finalDamage = damage;
            _finalObjectDamage = damage;
            if (playerBuffs == null) return; 

            playerBuffs.Attack
                .Subscribe(atkBonus => 
                {
                    _finalDamage = damage + Mathf.RoundToInt(atkBonus);
                })
                .AddTo(this); 

            playerBuffs.AttackForObject
                .Subscribe(objAtkBonus => 
                {
                    _finalObjectDamage = damage + Mathf.RoundToInt(objAtkBonus);
                })
                .AddTo(this);
        }
        public override void OnEquip()
        {
            pool ??= new Pool<SingleBullet>(initCount, bullet);
        }

        public override void OnUnEquip() { }

        public override void OnFireDown()
        {
            if (!CanShot()) return;
            SoundManager.Instance.PlaySE(SoundManager.SEData.SETYPE.ShingleShot);
            int finalDamage       = _finalDamage;
            int finalObjectDamage = _finalObjectDamage;
            Debug.Log($"最終攻撃力: {finalDamage}, 最終対物攻撃力: {finalObjectDamage}");

            var instance =  pool.GetPooledObject();
            instance.transform.position = this.transform.position + this.transform.forward*20;
            instance.Shot(CalcTargetPosition(),speed,finalDamage,finalObjectDamage,this.gameObject,photonView.IsMine,Distance);
            SetNextFireTime();
        }

        public override void OnFire() { }
        public override void OnFireUp() { }
    }
}