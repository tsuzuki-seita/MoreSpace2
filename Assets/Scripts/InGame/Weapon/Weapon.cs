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
        private RaycastHit[] hits = new RaycastHit[20];

        private void Start()
        {
            _mainCameraTransform = GetComponentInChildren<Camera>(true).gameObject.transform;
            playerBuffs = GetComponentInParent<PlayerBuffs>();
            InitializeBuffsAndSubscribe();
        }

        protected virtual void Update()
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
            int count = Physics.RaycastNonAlloc(cameraRay, hits, maxDistance);
            if(count == 0) return cameraRay.GetPoint(maxDistance);
            Array.Sort(hits, 0, count, System.Collections.Generic.Comparer<RaycastHit>.Create((a, b) => a.distance.CompareTo(b.distance)));

            foreach (var hit in hits)
            {
                // Triggerなら無視して次のヒットを確認
                if (hit.collider.isTrigger) continue;

                hitObject = hit.collider.gameObject;
                Debug.Log($"🎯 Raycast HIT: {hitObject.name}. Layer: {hitObject.layer}");
    
                return hit.point;
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