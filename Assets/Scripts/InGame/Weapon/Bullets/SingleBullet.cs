using System;
using ObjectPool;
using UnityEngine;

namespace MoreSpace.InGame.Weapons.Bullets
{
    [RequireComponent(typeof(Rigidbody))]
    public class SingleBullet : PooledObject
    {
        [SerializeField] private Rigidbody _rigidbody;
        private int _damage;
        private GameObject _ownerObject;
        private bool _isMine;
        public void Shot(Vector3 targetPosition, float speed, int damage ,GameObject ownerObject,bool isMine)
        {
            _ownerObject = ownerObject;
            _isMine = isMine;
            _damage = damage;
            this.transform.LookAt(targetPosition);
            _rigidbody.linearVelocity = transform.forward * speed;
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
                    damage.Damage(_damage);
            }

            Release();
        }
    }
}