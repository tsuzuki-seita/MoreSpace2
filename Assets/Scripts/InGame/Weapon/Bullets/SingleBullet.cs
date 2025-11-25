using System;
using ObjectPool;
using UnityEngine;

namespace MoreSpace.InGame.Weapons.Bullets
{
    [RequireComponent(typeof(Rigidbody))]
    public class SingleBullet : PooledObject
    {
        [SerializeField] private Rigidbody _rigidbody;
        private int _playerDamage;    // 敵機（PlayerHp）用
        private int _objectDamage;    // クリスタル（CrystalHealthなど）用
        private GameObject _ownerObject;
        private bool _isMine;
        public void Shot(Vector3 targetPosition, float speed, int finalPlayerDamage, int finalObjectDamage,GameObject ownerObject,bool isMine, float releaseTime)
        {
            _ownerObject = ownerObject;
            _isMine = isMine;
            _playerDamage = finalPlayerDamage;
            _objectDamage = finalObjectDamage;
            this.transform.LookAt(targetPosition);
            _rigidbody.linearVelocity = transform.forward * speed;
            Release(releaseTime);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (_isMine)
            {
                if (other.gameObject == _ownerObject)
                {
                    Debug.Log("自傷を防止したよ");
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