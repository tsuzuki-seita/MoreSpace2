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
        public void Shot(Vector3 targetPosition, float speed, int damage)
        {
            _damage = damage;
            this.transform.LookAt(targetPosition);
            _rigidbody.linearVelocity = transform.forward * speed;
        }

        private void OnCollisionEnter(Collision other)
        {
            if(other.gameObject.TryGetComponent<IDamageable>(out var damage))
                // damage.Damage(_damage);
            Release();
        }
    }
}