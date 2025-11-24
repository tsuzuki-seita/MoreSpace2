using ObjectPool;
using UnityEngine;
using MoreSpace.InGame.Weapons.Bullets;
using R3;

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
        
        [SerializeField] public int ObjectDamage = 10;
        private int _finalDamage = 0;
        private int _finalObjectDamage = 0;
        
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