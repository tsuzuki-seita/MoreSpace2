using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using ObjectPool;
using UnityEngine;

namespace MoreSpace.InGame.Weapons.Bullets
{
    [RequireComponent(typeof(Rigidbody))]
    public class HomingBullet : PooledObject
    {
        [Header("Homing Settings")]
        [SerializeField] private float _rotateSpeed = 5f; // 追尾の旋回性能
        [SerializeField] private Rigidbody _rigidbody;
        
        private int _playerDamage;
        private int _objectDamage;
        private GameObject _ownerObject;
        private bool _isMine;

        // 追尾用
        private GameObject _target;
        private float _speed;
        [SerializeField] private MeshRenderer bulletMesh;
        [SerializeField] private float destroyParticleTime = 1f;

        [Header("爆発用　不要ならnullでOK")] [SerializeField] private MissileExplosion explosion;
        private static Pool<MissileExplosion> explosionPool;
        private bool isCollisioned;
        private Vector3 stopPosition;
        //爆発同期用
        private int _bulletId;
        private Action<int, Vector3> _onHitCallback;

        // 引数に target (GameObject) を追加
        public void Shot(int bulletId, Action<int, Vector3> onHitCallback,Vector3 targetPosition, float speed, int finalPlayerDamage, int finalObjectDamage, GameObject ownerObject, bool isMine, float releaseTime, GameObject target = null)
        {
            if(explosion != null && explosionPool == null)
                InitializeExplosionPool();
            
            _bulletId = bulletId;
            _onHitCallback = onHitCallback;
            _ownerObject = ownerObject;
            _isMine = isMine;
            _playerDamage = finalPlayerDamage;
            _objectDamage = finalObjectDamage;
            isCollisioned = false;
            
            _target = target; // ターゲットを保存（nullなら直進モード）
            _speed = speed;

            // 初期位置での向き設定
            if (_target != null)
            {
                Debug.Log("ターゲットあり");
                // ターゲットがいるならそっちを向く
                transform.LookAt(_target.transform);
            }
            else
            {
                // ターゲットがいない（またはnull指定）なら指定座標の方を向く（従来の挙動）
                transform.LookAt(targetPosition);
            }

            // 初速を与える
            _rigidbody.linearVelocity = transform.forward * _speed;
            Invoke(nameof(Release),releaseTime);
        }

        void InitializeExplosionPool()
        {
            explosionPool = new Pool<MissileExplosion>(0, explosion);
        }

        private void FixedUpdate()
        {
            // ターゲットが存在する場合（破壊されてnullになったら自動的に直進になる）
            if (_target != null)
            {
                Vector3 direction = _target.transform.position - transform.position;
                if (direction != Vector3.zero)
                {
                    Debug.Log("旋回します");
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    // 指定した旋回速度で徐々に向きを変える
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * _rotateSpeed);
                }
            }

            // 毎フレーム速度ベクトルを更新（回転しても前進し続けるため）
            _rigidbody.linearVelocity = transform.forward * _speed;
            if (isCollisioned) transform.position = stopPosition;
        }

        private void OnCollisionEnter(Collision other)
        {
            stopPosition = transform.position;
            if (isCollisioned) return;
            if (!_isMine) return;
            
            isCollisioned = true;

            if (other.gameObject != _ownerObject && other.gameObject.TryGetComponent<IDamageable>(out var damage)) 
            {
                int appliedDamage = other.gameObject.GetComponent<CrystalHealth>() != null ? _objectDamage : _playerDamage;
                damage.Damage(appliedDamage);
            }
            
            _onHitCallback?.Invoke(_bulletId, this.transform.position);
        }
        
        public async void NetworkExplode(Vector3 hitPosition)
        {
            if (!bulletMesh.enabled) return;
            
            isCollisioned = true;
            _speed = 0;
            _rigidbody.linearVelocity = Vector3.zero;

            if (explosion != null)
                _ = explosionPool.GetPooledObject().Explosion(hitPosition, _playerDamage, _objectDamage, _ownerObject, _isMine);
            
            await WaitTrail();
        }

        private async Task WaitTrail()
        {
            bulletMesh.enabled = false;
            _speed = 0;
            await UniTask.WaitForSeconds(destroyParticleTime);
            bulletMesh.enabled = true;
            _target = null;
            Release();
        }
    }
}