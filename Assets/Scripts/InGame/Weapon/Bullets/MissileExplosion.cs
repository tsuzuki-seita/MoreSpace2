using Cysharp.Threading.Tasks;
using ObjectPool;
using UnityEngine;

namespace MoreSpace.InGame.Weapons.Bullets
{
    public class MissileExplosion : PooledObject
    {
        [SerializeField] private ParticleSystem explosion;
        private int _playerDamage;
        private int _objectDamage;
        private GameObject _ownerObject;
        private bool _isMine;

        public async UniTask Explosion(Vector3 position,int finalPlayerDamage, int finalObjectDamage, GameObject ownerObject, bool isMine)
        {
            _ownerObject = ownerObject;
            _isMine = isMine;
            _playerDamage = finalPlayerDamage;
            _objectDamage = finalObjectDamage;
            
            transform.position = position;
            explosion.Play();
            await UniTask.WaitForSeconds(1, cancellationToken: this.GetCancellationTokenOnDestroy());
            Release();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (_isMine)
            {
                if (other.gameObject == _ownerObject)
                {
                    return;
                }

                if (other.gameObject.TryGetComponent<IDamageable>(out var damage)) 
                {
                    int appliedDamage = other.gameObject.GetComponent<CrystalHealth>() != null ? _objectDamage : _playerDamage;
                    damage.Damage(appliedDamage);
                }
            }
        }
    }
}