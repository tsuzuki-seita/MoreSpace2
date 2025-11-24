using System.Linq;
using MoreSpace.InGame.Player;
using ObjectPool;
using UnityEngine;
using MoreSpace.InGame.Weapons.Bullets;
using R3;

namespace MoreSpace.InGame.Weapons
{
    public class HomingShot : Weapon
    {
        [SerializeField] public HomingBullet bullet;
        [SerializeField] public uint initCount = 5;
        [SerializeField] public float speed = 10;
        [SerializeField] public int damage = 10;

        [SerializeField] public int ObjectDamage = 10;
        [SerializeField] public bool isHomingAlways;

        private Pool<HomingBullet> pool;
        private int _finalDamage = 0;
        private int _finalObjectDamage = 0;
        
        private Camera cam;
        private GameObject enemyObject;
        
        protected override void InitializeBuffsAndSubscribe()
        {
            _finalDamage = damage;
            _finalObjectDamage = ObjectDamage;
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
                    _finalObjectDamage = ObjectDamage + Mathf.RoundToInt(objAtkBonus);
                })
                .AddTo(this);
        }
        public override void OnEquip()
        {
            pool ??= new Pool<HomingBullet>(initCount, bullet);
            cam ??= gameObject.GetComponentInChildren<Camera>();
            enemyObject ??= PlayerObjectHolder.Instance.player.FirstOrDefault(p => !p.IsMine)?.gameObject;
        }

        public override void OnUnEquip() { }

        public override void OnFireDown()
        {
            if (!CanShot()) return;
            int finalDamage       = _finalDamage;
            int finalObjectDamage = _finalObjectDamage;
            Debug.Log($"最終攻撃力: {finalDamage}, 最終対物攻撃力: {finalObjectDamage}");

            var instance =  pool.GetPooledObject();
            instance.transform.position = this.transform.position + this.transform.forward*30;
            instance.Shot(CalcTargetPosition(),speed,finalDamage,finalObjectDamage,this.gameObject,photonView.IsMine,CheckEnemyInCamera());
            SetNextFireTime();
        }
        
        private GameObject CheckEnemyInCamera()
        {
            if (enemyObject == null) return null;
            if (isHomingAlways) return enemyObject;
            var vp = cam.WorldToViewportPoint(enemyObject.transform.position);
            
            // 画面内(0-1) かつ カメラの前方(z>0) か
            bool isVisible = vp.x is > 0f and < 1f && 
                             vp.y is > 0f and < 1f && 
                             (vp.z > 0f);

            if (isVisible) return enemyObject;
            else return null;
        }

        public override void OnFire() { }
        public override void OnFireUp() { }
    }
}