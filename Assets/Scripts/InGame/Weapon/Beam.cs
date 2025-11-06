using ObjectPool;
using UnityEngine;
using MoreSpace.InGame.Weapons.Bullets;
namespace MoreSpace.InGame.Weapons.Bullets
{
    public class Beam : Weapon
    {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private float range = 10;
        [SerializeField] private int damage = 10;
        
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
            var target = CheckHitObjectDamageable();
            if(target != null)
            {
                Debug.Log("Beam hit and damage applied.");
                target.Damage(damage);
            }
                
        }
        public override void OnFireUp() 
        {
            lineRenderer.enabled = false;
            SetNextFireTime();
        }
    }
}