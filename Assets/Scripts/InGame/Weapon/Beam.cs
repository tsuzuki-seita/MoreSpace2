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
        
        public override void OnEquip()
        {
            if(lineRenderer == null) lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.enabled = false;
        }

        public override void OnUnEquip() { }

        public override void OnFireDown()
        {
            lineRenderer.enabled = true;
        }

        public override void OnFire()
        {
            if(!CanShot()) return;
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
        }
    }
}