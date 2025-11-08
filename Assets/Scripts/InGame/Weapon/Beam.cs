using ObjectPool;
using UnityEngine;
using MoreSpace.InGame.Weapons.Bullets;
namespace MoreSpace.InGame.Weapons.Bullets
{
    public class Beam : Weapon
    {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] public float range = 10;
        [SerializeField] public int damage = 10;
        [SerializeField] public float timerMax = 1;
        private float _timer = 0;
        [SerializeField] public float maxBeamDuration = 3.0f;
        [SerializeField] public float cooldownTime = 5.0f;
        private float _currentBeamDuration = 0f;
        private float _cooldownTimer = 0f;
        private bool _canFireBeam = true;
        
        public override void OnEquip()
        {
            if(lineRenderer == null) lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.enabled = false;
            _currentBeamDuration = 0f; 
            _cooldownTimer = 0f;
            _canFireBeam = true;
        }

        public override void OnUnEquip() { }

        public override void OnFireDown()
        {
            if (!_canFireBeam) return;
            lineRenderer.enabled = true;
            _currentBeamDuration = 0f;
        }

        public override void OnFire()
        {
            if (!CanShot()) return;
            _currentBeamDuration += Time.deltaTime;
            if (_currentBeamDuration >= maxBeamDuration)
            {
                Debug.Log("もう打てないよ");
                OnFireUp(); 
                return;
            }
            lineRenderer.SetPosition(0,transform.position);
            lineRenderer.SetPosition(1,CalcTargetPosition());
            
            if(!photonView.IsMine) return;
            
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
            lineRenderer.enabled = false;
            SetNextFireTime();
            if (_currentBeamDuration > 0f)
            {
                _canFireBeam = false;
                _cooldownTimer = cooldownTime;
            }
            _currentBeamDuration = 0f;
        }
        private void Update() 
        {
            if (!_canFireBeam)
            {
                _cooldownTimer -= Time.deltaTime;
                if (_cooldownTimer <= 0)
                {
                    _canFireBeam = true; 
                    Debug.Log("打てるようになったよ");
                }
            }
        }
    }
}