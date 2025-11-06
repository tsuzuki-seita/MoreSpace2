using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace MoreSpace.InGame.Weapons
{
    public abstract class Weapon : MonoBehaviourPunCallbacks
    {
        [SerializeField] protected float fireRate = 0.25f;
        [SerializeField] protected float maxDistance = 100;
        private GameObject hitObject;
        private float _nextFireTime = 0;
        private Transform _mainCameraTransform;

        private void Start()
        {
            _mainCameraTransform = GetComponentInChildren<Camera>(true).gameObject.transform;
        }

        protected bool CanShot()
        {
            return Time.time > _nextFireTime;
        }

        protected void SetNextFireTime()
        {
            _nextFireTime = Time.time + fireRate;
        }

        public abstract void OnEquip();
        public abstract void OnUnEquip();
        public abstract void OnFireDown();
        public abstract void OnFire();
        public abstract void OnFireUp();

        protected Vector3 CalcTargetPosition()
        {
            Ray cameraRay = new Ray(_mainCameraTransform.position, _mainCameraTransform.forward);
            if (Physics.Raycast(cameraRay, out var result,maxDistance))
            {
                hitObject = result.collider.gameObject;
                Debug.Log($"🎯 Raycast HIT: {hitObject.name}. Layer: {hitObject.layer}");
                return result.point;
            }

            hitObject = null;
            return cameraRay.GetPoint(maxDistance);
        }

        protected IDamageable CheckHitObjectDamageable()
        {
            IDamageable result = null;
            if (hitObject != null)
            {
                result = hitObject.GetComponentInParent<IDamageable>();
            }
            Debug.Log($"Hit Object Damageable: {result != null}");
            return result;
        }
    }
}