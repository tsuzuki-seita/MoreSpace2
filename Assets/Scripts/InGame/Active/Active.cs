using UnityEngine;
using Photon.Pun;

namespace MoreSpace.InGame.Weapons
{
    // Active系の共通ロジック（リキャスト・効果時間・排他制御）
    public abstract class Active : Weapon
    {
        [Header("Active Config (Skillデータから代入される)")]
        public float Duration   = 5f;

        protected float _currentDuration = 0f;
        protected bool  _isActive        = false;

        // --- 時間管理（自分専用のCD/Durationだけ進める） ---
        protected override void Update()
        {
            base.Update();
            // 効果時間中
            if (_isActive)
            {
                _currentDuration -= Time.deltaTime;
                if (_currentDuration <= 0f)
                {
                    StopActiveLocal();
                }
            }
        }

        // --- 発動ボタン押した瞬間（ControlWeapon から全クライアントで呼ばれる） ---
        public override void OnFireDown()
        {
            // リキャスト中なら無視
            if (!CanShot()) return;

            // すでに発動中なら無視
            if (_isActive) return;

            // ここでローカル発動（RPCは不要）
            StartActiveLocal();

            SetNextFireTime();
        }

        // 押しっぱなし／離した瞬間はActiveでは特に使わない
        public override void OnFire() { }
        public override void OnFireUp() { }

        // --- 武器切り替え時 ---
        public override void OnUnEquip()
        {
            // ここでは何もしない
            // → 「武器/Activeを切り替えただけでは効果を切らない」という仕様
        }

        public override void OnEquip()
        {
            // 必要ならUI更新など
        }

        // ===== 共通の開始/終了ロジック =====

        /// <summary>実際の発動開始処理（RPC不要、全クライアントで同じことをする）</summary>
        protected void StartActiveLocal()
        {
            // 1) 自分以外で「発動中のActive」があれば止めてリキャストに入れる
            var actives = GetComponents<Active>();
            foreach (var other in actives)
            {
                if (other == this) continue;
                if (!other._isActive) continue;

                other.StopActiveLocal();  // この中でそのActiveのリキャスト開始
            }

            // 2) 自分を発動状態にする
            _isActive        = true;
            _currentDuration = Duration;

            OnActivateStart();
        }

        /// <summary>実際の終了処理（Duration切れ or 他のActive発動時）</summary>
        protected void StopActiveLocal()
        {
            if (!_isActive) return;

            _isActive        = false;
            _currentDuration = 0f;

            OnActivateStop();
        }

        /// <summary>派生クラスで：効果開始時の処理（見た目やフラグ）</summary>
        protected abstract void OnActivateStart();

        /// <summary>派生クラスで：効果終了時の処理（見た目やフラグ解除）</summary>
        protected abstract void OnActivateStop();
    }
}
