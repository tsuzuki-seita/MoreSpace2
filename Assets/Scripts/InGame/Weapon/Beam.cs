using ObjectPool;
using UnityEngine;
using MoreSpace.InGame.Weapons.Bullets;
namespace MoreSpace.InGame.Weapons
{
    public class Beam : Weapon
    {
        public BeamBullet bulletPrefab;
        [SerializeField] public int damage = 10;
        [SerializeField] public float maxBeamDuration = 3.0f;
        [SerializeField] private float timerMax = 1f;
        private float _timer = 0;
        private float _currentBeamDuration = 0f;
        private bool isReseted;
        private BeamBullet bullet;
        
        public override void OnEquip()
        {
            if (bullet == null)
            {
                bullet = Instantiate(bulletPrefab, this.transform);
                bullet.transform.localPosition = Vector3.zero;
            }
            bullet.gameObject.SetActive(false);
            _currentBeamDuration = 0f; 
        }

        public override void OnUnEquip() { }

        public override void OnFireDown()
        {
            if (!CanShot()) return;
            bullet.gameObject.SetActive(true);
            _currentBeamDuration = 0f;
            isReseted = false;
        }

        public override void OnFire()
        {
            if (!CanShot()) return;
            bullet.Visualize(transform.position,CalcTargetPosition());
            
            if(!photonView.IsMine) return;
            
            _currentBeamDuration += Time.deltaTime;
            if (_currentBeamDuration >= maxBeamDuration)
            {
                OnForceStop?.Invoke();
                return;
            }
            _timer += Time.deltaTime;
            if (_timer > timerMax)
            {
                var target = CheckHitObjectDamageable();
                target?.Damage(damage);
                _timer = 0;
            }
        }
        public override void OnFireUp()
        {
            if(isReseted) return;
            bullet.gameObject.SetActive(false);
            isReseted = true;
            SetNextFireTime();
            _currentBeamDuration = 0f;
        }
    }
}