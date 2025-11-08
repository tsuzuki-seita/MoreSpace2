using Photon.Pun;
using UnityEngine;

namespace MoreSpace.InGame.Weapons
{
    [DisallowMultipleComponent]
    public abstract class Active : Weapon
    {
        [Header("Active Config (Scriptableから代入)")]
        public float RecastTime = 10f;
        public float Duration   = 5f;

        [SerializeField] protected float _currentRecast   = 0f;
        [SerializeField] protected float _currentDuration = 0f;
        [SerializeField] protected bool  _isActive        = false;

        /// <summary>外部（例: PlayerHP）から見える発動状態</summary>
        public bool IsActive => _isActive;

        protected virtual void Update()
        {
            if (_currentRecast > 0f)   _currentRecast  -= Time.deltaTime;

            if (_isActive)
            {
                _currentDuration -= Time.deltaTime;
                if (_currentDuration <= 0f)
                {
                    if (photonView && photonView.IsMine)
                        photonView.RPC(nameof(RPC_StopActive), RpcTarget.All);
                    else
                        StopActiveLocal();
                }
            }
        }

        // ===== Weapon 標準インターフェイス実装 =====

        public override void OnEquip()
        {
            // 必要ならVFXやUIの初期化など
        }

        public override void OnUnEquip()
        {
            // 装備解除時は効果が残らないように停止
            if (_isActive)
            {
                if (photonView && photonView.IsMine)
                    photonView.RPC(nameof(RPC_StopActive), RpcTarget.All);
                else
                    StopActiveLocal();
            }
        }

        public override void OnFireDown()
        {
            TryActivate();
        }

        public override void OnFire()   { /* 押しっぱなし不要 */ }
        public override void OnFireUp() { /* 何もしない      */ }

        // ===== 発動系 =====

        /// <summary>発動を試みる（クールダウン中や発動中は失敗）</summary>
        public bool TryActivate()
        {
            if (_isActive)        return false;
            if (_currentRecast > 0f) return false;
            if (!CanShot())       return false; // Weapon.fireRate による連打抑制

            if (photonView && photonView.IsMine)
                photonView.RPC(nameof(RPC_StartActive), RpcTarget.All);
            else
                StartActiveLocal();

            SetNextFireTime(); // Weapon.fireRate を尊重（入力スパム抑制）
            return true;
        }

        [PunRPC] protected void RPC_StartActive() => StartActiveLocal();
        [PunRPC] protected void RPC_StopActive()  => StopActiveLocal();

        protected virtual void StartActiveLocal()
        {
            _isActive        = true;
            _currentDuration = Duration;
            _currentRecast   = RecastTime;
            OnActivateStart();
        }

        protected virtual void StopActiveLocal()
        {
            _isActive        = false;
            _currentDuration = 0f;
            OnActivateStop();
        }

        /// <summary>派生クラスでの効果開始処理</summary>
        protected abstract void OnActivateStart();

        /// <summary>派生クラスでの効果終了処理</summary>
        protected abstract void OnActivateStop();
    }
}
