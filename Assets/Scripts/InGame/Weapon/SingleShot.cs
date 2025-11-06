using ObjectPool;
using UnityEngine;
using MoreSpace.InGame.Weapons.Bullets;

namespace MoreSpace.InGame.Weapons
{
    public class SingleShot : Weapon
    {
        [SerializeField] private SingleBullet bullet;
        [SerializeField] private uint initCount = 10;
        [SerializeField] private float speed = 10;
        [SerializeField] private int damage = 10;

        private Pool<SingleBullet> pool;
        
        public override void OnEquip()
        {
            pool ??= new Pool<SingleBullet>(initCount, bullet);
        }

        public override void OnUnEquip() { }

        public override void OnFireDown()
        {
            if(!CanShot()) return;
            var instance =  pool.GetPooledObject();
            instance.transform.position = this.transform.position + this.transform.forward*20;
            instance.Shot(CalcTargetPosition(),speed,damage,this.gameObject,photonView.IsMine);
            SetNextFireTime();
        }

        public override void OnFire() { }
        public override void OnFireUp() { }
    }
}