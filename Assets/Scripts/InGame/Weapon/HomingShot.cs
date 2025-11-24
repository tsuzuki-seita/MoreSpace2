using System.Collections.Generic;
using System.Linq;
using MoreSpace.InGame.Player;
using ObjectPool;
using UnityEngine;
using MoreSpace.InGame.Weapons.Bullets;
using Photon.Pun;
using R3;

namespace MoreSpace.InGame.Weapons
{
    public class HomingShot : Weapon
    {
        [SerializeField] public HomingBullet bullet;
        [SerializeField] public uint initCount = 5;
        [SerializeField] public float speed = 10;
        [SerializeField] public int damage = 10;

        [SerializeField] public bool isHomingAlways;

        private Pool<HomingBullet> pool;
        private int _finalDamage = 0;
        private int _finalObjectDamage = 0;
        
        private Camera cam;
        private GameObject enemyObject;
        
        // 追加: アクティブな弾を管理する辞書とカウンター
        private Dictionary<int, HomingBullet> _activeBullets = new Dictionary<int, HomingBullet>();
        private int _bulletIdCounter = 0;
        
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
            pool ??= new Pool<HomingBullet>(initCount, bullet);
            cam ??= gameObject.GetComponentInChildren<Camera>();
            enemyObject ??= PlayerObjectHolder.Instance.player.FirstOrDefault(p => !p.Value.IsMine).Value?.gameObject;
        }

        public override void OnUnEquip() { }

        public override void OnFireDown()
        {
            if (!CanShot()) return;
            int finalDamage       = _finalDamage;
            int finalObjectDamage = _finalObjectDamage;
            Debug.Log($"最終攻撃力: {finalDamage}, 最終対物攻撃力: {finalObjectDamage}");
            
            var targetPosition = CalcTargetPosition();
            var instance =  pool.GetPooledObject();
            
            int id = _bulletIdCounter++;
            _activeBullets[id] = instance;

            instance.transform.position = this.transform.position + this.transform.forward*30;
            instance.transform.LookAt(targetPosition);
            instance.Shot(id,OnBulletHit,targetPosition,speed,finalDamage,finalObjectDamage,this.gameObject,photonView.IsMine,CheckEnemyInCamera());
            SetNextFireTime();
        }
        
        // 弾から「当たった」と連絡が来るメソッド（Ownerのみ実行）
        private void OnBulletHit(int bulletId, Vector3 hitPosition)
        {
            // 全員に「弾を消せ」と命令
            photonView.RPC(nameof(RPC_ReleaseBullet), RpcTarget.All, bulletId, hitPosition);
        }

        [PunRPC]
        private void RPC_ReleaseBullet(int bulletId, Vector3 hitPosition)
        {
            if (_activeBullets.TryGetValue(bulletId, out var targetBullet))
            {
                targetBullet.NetworkExplode(hitPosition);
                _activeBullets.Remove(bulletId);
            }
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
            return null;
        }

        public override void OnFire() { }
        public override void OnFireUp() { }
    }
}