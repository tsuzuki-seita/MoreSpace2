using DG.Tweening; // DOTweenの名前空間を追加
using Photon.Pun;
using UnityEngine;

namespace MoreSpace.InGame.Player
{
    public class VisualizeWireOnAttackCrystal : MonoBehaviourPunCallbacks
    {
        [SerializeField] private MeshRenderer wireRenderer;
        [SerializeField] private float visualizeTime = 1.5f;
        [SerializeField] private float fadeDuration = 0.25f; // フェードにかかる時間

        private int propertyID;
        private string frameColorPropertyName = "_WireframeColor";

        private Material _cachedMaterial;
        private Color _targetColor;      // 表示時の色（Alphaあり）
        private Color _transparentColor; // 非表示時の色（Alpha 0）
        private Tween _currentTween;     // 現在実行中のアニメーション

        void Start()
        {
            propertyID = Shader.PropertyToID(frameColorPropertyName);
            _cachedMaterial = wireRenderer.material;

            Color baseColor = !photonView.IsMine ? new Color32(255, 0, 0, 255) : new Color32(255, 0, 0, 50);
            
            _targetColor = baseColor;
            _transparentColor = baseColor;
            _transparentColor.a = 0f;

            // 初期状態設定
            wireRenderer.enabled = false;
            _cachedMaterial.SetColor(propertyID, _transparentColor);
        }

        public void StartVisualize()
        {
            // 既にアニメーション中ならキャンセルしてリセット（連打対応）
            if (_currentTween != null && _currentTween.IsActive())
            {
                _currentTween.Kill();
            }

            VisualizeSequence();
        }

        void VisualizeSequence()
        {
            wireRenderer.enabled = true;
            
            // 初期色を透明にセット（念のため）
            _cachedMaterial.SetColor(propertyID, _transparentColor);

            Sequence seq = DOTween.Sequence();
            seq.Append(_cachedMaterial.DOColor(_targetColor, propertyID, fadeDuration));
            float waitTime = Mathf.Max(0, visualizeTime - (fadeDuration * 2));
            seq.AppendInterval(waitTime);
            seq.Append(_cachedMaterial.DOColor(_transparentColor, propertyID, fadeDuration));

            // 4. 終了時の処理
            seq.OnComplete(() =>
            {
                wireRenderer.enabled = false;
                _currentTween = null;
            });

            _currentTween = seq;
        }

        // オブジェクト破棄時にTweenを安全にキルする
        private void OnDestroy()
        {
            if (_currentTween != null && _currentTween.IsActive())
            {
                _currentTween.Kill();
            }
            
            // マテリアルインスタンスの破棄（メモリリーク防止）
            if (_cachedMaterial != null)
            {
                Destroy(_cachedMaterial);
            }
        }
    }
}