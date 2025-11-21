using System;
using DG.Tweening;
using UnityEngine;

namespace MoreSpace.InGame.Player
{
    public class DamageEffect : MonoBehaviour
    {
        [SerializeField] private HealthBase playerHealth;
        [SerializeField] private ParticleSystem damageParticle;
        [SerializeField] private GameObject camObject;
        [SerializeField] private GameObject damageUi;
        
        [Header("ダメージを示す煙")] [SerializeField] private int maxDamageParticleCount = 50;
        
        [Header("シェイクの基本設定")]
        [SerializeField] private float baseShakeDuration = 0.5f;   // 基本の揺れ時間
        [SerializeField] private float baseShakeStrength = 0.5f;   // 基本の揺れの振幅
        [SerializeField] private int baseShakeVibrato = 10;        // 振動数 (揺れの細かさ)
        [SerializeField] private float baseShakeRandomness = 90f;  // 揺れのランダム性

        [Header("連続ダメージ時の設定")]
        [SerializeField] private float continuousDamageIncreaseStrength = 0.2f; // 連続ダメージで振幅を増やす量
        [SerializeField] private float maxShakeStrength = 2.0f; // 最大の揺れの振幅

        private Tween _currentShakeTween; // 現在実行中のシェイクTweenを保持
        private float _currentShakeStrength; // 現在の揺れの振幅
        private bool _isShaking = false; // 現在カメラが揺れているか
        private ParticleSystem.EmissionModule _module;
        
        private void Start()
        {
            _currentShakeStrength = baseShakeStrength;
            playerHealth.OnDamage += ShakeAndAlertOnDamage;
            playerHealth.OnDamage += ParticleOnDamage;
            //初期化
            damageUi.SetActive(false);
            _module = damageParticle.emission;
            SetDamageParticleEmit(0);
        }

        void SetDamageParticleEmit(int count)
        {
            var rate = _module.rateOverTime;
            rate.constant = count;
            _module.rateOverTime = rate;
        }

        void ParticleOnDamage(int hp, int maxHp)
        {
            SetDamageParticleEmit(Mathf.FloorToInt(maxDamageParticleCount * (float)(maxHp - hp) / maxHp));
        }

        void ShakeAndAlertOnDamage(int hp, int maxHp)
        {
            // 既に揺れている場合
            if (_isShaking)
            {
                // 既存の揺れをキャンセル（これにより、新しい揺れがスムーズに開始される）
                _currentShakeTween?.Kill(true); // trueを渡すと、現在の状態を終了地点として適用
                // 振幅を増やす
                _currentShakeStrength = Mathf.Min(_currentShakeStrength + continuousDamageIncreaseStrength, maxShakeStrength);
            }
            else
            {
                // 新しく揺れ始める場合
                _currentShakeStrength = baseShakeStrength;
                damageUi.SetActive(true);
            }

            _isShaking = true;

            // カメラのPositionを揺らす (Z軸方向はあまり揺らさない方が良いことが多い)
            _currentShakeTween = transform.DOShakePosition(
                    baseShakeDuration, 
                    new Vector3(_currentShakeStrength, _currentShakeStrength, 0), // X, Y軸方向の振幅を現在の強度に設定
                    baseShakeVibrato, 
                    baseShakeRandomness
                )
                .SetEase(Ease.OutQuad) // 揺れの減衰カーブ
                .OnComplete(() => {
                    // 揺れが完了したらフラグをリセットし、振幅も基本値に戻す
                    _isShaking = false;
                    _currentShakeStrength = baseShakeStrength; // 揺れが終わったら振幅をリセット
                    damageUi.SetActive(false);
                })
                .Play();
        }

        private void OnDestroy()
        {
            playerHealth.OnDamage -= ShakeAndAlertOnDamage;
            playerHealth.OnDamage -= ParticleOnDamage;
        }
    }
}