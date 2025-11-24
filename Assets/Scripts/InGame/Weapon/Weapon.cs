using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using R3;

namespace MoreSpace.InGame.Weapons
{
    public abstract class Weapon : MonoBehaviourPunCallbacks
    {
        public float fireRate = 0.25f;
        public Action OnForceStop;
        public ReactiveProperty<float> nextFireTime { get; private set; } = new ReactiveProperty<float>(0);
        [SerializeField] public float maxDistance = 10000;
        [SerializeField] public PlayerBuffs playerBuffs;
        private GameObject hitObject;
        private Transform _mainCameraTransform;

        private void Start()
        {
            _mainCameraTransform = GetComponentInChildren<Camera>(true).gameObject.transform;
            playerBuffs = GetComponentInParent<PlayerBuffs>();
            InitializeBuffsAndSubscribe();
        }

        private void Update()
        {
            nextFireTime.Value = Mathf.Clamp(nextFireTime.Value - Time.deltaTime, 0, Mathf.Infinity);
        }

        protected bool CanShot()
        {
            return nextFireTime.Value == 0;
        }

        protected void SetNextFireTime()
        {
            nextFireTime.Value = fireRate;
        }
        protected virtual void InitializeBuffsAndSubscribe() { }
        public abstract void OnEquip();
        public abstract void OnUnEquip();
        public abstract void OnFireDown();
        public abstract void OnFire();
        public abstract void OnFireUp();

        protected Vector3 CalcTargetPosition()
        {
            Ray cameraRay = new Ray(_mainCameraTransform.position, _mainCameraTransform.forward);
            if (Physics.Raycast(cameraRay, out var result, maxDistance))
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