using System;
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

        // 引数に target (GameObject) を追加
        public void Shot(Vector3 targetPosition, float speed, int finalPlayerDamage, int finalObjectDamage, GameObject ownerObject, bool isMine, GameObject target = null)
        {
            _ownerObject = ownerObject;
            _isMine = isMine;
            _playerDamage = finalPlayerDamage;
            _objectDamage = finalObjectDamage;
            
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
        }

        private void OnCollisionEnter(Collision other)
        {
            if (_isMine)
            {
                if (other.gameObject == _ownerObject)
                {
                    // Debug.Log("自傷を防止したよ");
                    Release();
                    return;
                }

                if (other.gameObject.TryGetComponent<IDamageable>(out var damage)) 
                {
                    int appliedDamage = other.gameObject.GetComponent<CrystalHealth>() != null ? _objectDamage : _playerDamage;
                    damage.Damage(appliedDamage);
                }
            }

            Release();
        }
    }
}