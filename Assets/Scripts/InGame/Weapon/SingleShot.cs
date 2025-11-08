using ObjectPool;
using UnityEngine;
using MoreSpace.InGame.Weapons.Bullets;

namespace MoreSpace.InGame.Weapons
{
    public class SingleShot : Weapon
    {
        [SerializeField] public SingleBullet bullet;
        [SerializeField] public uint initCount = 10;
        [SerializeField] public float speed = 10;
        [SerializeField] public int damage = 10;

        [SerializeField] public int ObjectDamage = 0;

        private Pool<SingleBullet> pool;
        private PlayerBuffs _buffs;
        
        public override void OnEquip()
        {
            pool ??= new Pool<SingleBullet>(initCount, bullet);
        }

        public override void OnUnEquip() { }

        public override void OnFireDown()
        {
            if (!CanShot()) return;
            
            // 1) バフの取得（なければ 0）
            float atkBonus    = _buffs != null ? _buffs.Attack          : 0f;
            float objAtkBonus = _buffs != null ? _buffs.AttackForObject : 0f;

            // 2) 最終ダメージを算出（加算）
            int finalDamage       = damage       + Mathf.RoundToInt(atkBonus);
            int finalObjectDamage = ObjectDamage + Mathf.RoundToInt(objAtkBonus);

            var instance =  pool.GetPooledObject();
            instance.transform.position = this.transform.position + this.transform.forward*20;
            instance.Shot(CalcTargetPosition(),speed,finalDamage,this.gameObject,photonView.IsMine);
            SetNextFireTime();
        }

        public override void OnFire() { }
        public override void OnFireUp() { }
    }
}