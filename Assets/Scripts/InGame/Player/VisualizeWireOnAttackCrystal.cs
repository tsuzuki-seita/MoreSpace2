using DG.Tweening;
using Photon.Pun;
using UnityEngine;

namespace MoreSpace.InGame.Player
{
    /// <summary>
    /// クリスタル攻撃時に、機体の Wireframe_Front マテリアルを
    /// 「敵からだけ」見えるようにフェード表示するコンポーネント。
    /// </summary>
    public class VisualizeWireOnAttackCrystal : MonoBehaviourPunCallbacks
    {
        [SerializeField] private MeshRenderer wireRenderer;
        [SerializeField] private float visualizeTime = 1.5f;
        [SerializeField] private float fadeDuration = 0.25f;

        // 新しいシェーダー側のプロパティ
        private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");
        private static readonly int XRayColorID = Shader.PropertyToID("_XRayColor");

        private Material _cachedMaterial;

        private Color _visibleBaseColor;
        private Color _visibleXRayColor;
        private Color _hiddenBaseColor;
        private Color _hiddenXRayColor;

        private Tween _currentTween;

        private void Start()
        {
            // 個別インスタンスをキャッシュ（他プレイヤーとマテリアルを共有しないように）
            _cachedMaterial = wireRenderer.material;

            if (photonView.IsMine)
            {
                // 自機側では基本見せたくないので、アルファ 0 にしておく
                _visibleBaseColor = new Color(1f, 0f, 0f, 0f);
                _visibleXRayColor = new Color(0f, 1f, 0f, 0f);
            }
            else
            {
                // 敵機はしっかり見えるように
                _visibleBaseColor = new Color(1f, 0f, 0f, 0.3f);  // うっすら赤
                _visibleXRayColor = new Color(0f, 1f, 0f, 0.9f);  // 奥側は強い緑
            }

            _hiddenBaseColor = _visibleBaseColor;
            _hiddenBaseColor.a = 0f;
            _hiddenXRayColor = _visibleXRayColor;
            _hiddenXRayColor.a = 0f;

            // 初期状態は非表示
            wireRenderer.enabled = false;
            _cachedMaterial.SetColor(BaseColorID, _hiddenBaseColor);
            _cachedMaterial.SetColor(XRayColorID, _hiddenXRayColor);
        }

        /// <summary>
        /// クリスタルに攻撃がヒットしたタイミングで呼ぶ。
        /// 所有者側から呼ばれたら、敵クライアントにだけ RPC で可視化命令を飛ばす。
        /// </summary>
        public void StartVisualize()
        {
            if (photonView.IsMine)
            {
                // 自機のクライアント → 敵クライアントだけに通知
                photonView.RPC(nameof(RpcStartVisualize), RpcTarget.Others);
            }
            else
            {
                // 何らかの理由で他人側から直接呼ばれた場合も一応対応
                RpcStartVisualize();
            }
        }

        [PunRPC]
        private void RpcStartVisualize()
        {
            // 既にアニメ中なら止める（連打対策）
            if (_currentTween != null && _currentTween.IsActive())
            {
                _currentTween.Kill();
            }

            VisualizeSequence();
        }

        private void VisualizeSequence()
        {
            wireRenderer.enabled = true;

            // 開始時は透明からスタート
            _cachedMaterial.SetColor(BaseColorID, _hiddenBaseColor);
            _cachedMaterial.SetColor(XRayColorID, _hiddenXRayColor);

            var seq = DOTween.Sequence();

            // フェードイン（手前部分の色 + 奥の X-Ray 部分の色を同時に）
            seq.Append(_cachedMaterial.DOColor(_visibleBaseColor, BaseColorID, fadeDuration));
            seq.Join(_cachedMaterial.DOColor(_visibleXRayColor, XRayColorID, fadeDuration));

            // 一定時間キープ
            float waitTime = Mathf.Max(0f, visualizeTime - fadeDuration * 2f);
            seq.AppendInterval(waitTime);

            // フェードアウト
            seq.Append(_cachedMaterial.DOColor(_hiddenBaseColor, BaseColorID, fadeDuration));
            seq.Join(_cachedMaterial.DOColor(_hiddenXRayColor, XRayColorID, fadeDuration));

            seq.OnComplete(() =>
            {
                wireRenderer.enabled = false;
                _currentTween = null;
            });

            _currentTween = seq;
        }

        private void OnDestroy()
        {
            if (_currentTween != null && _currentTween.IsActive())
            {
                _currentTween.Kill();
            }

            if (_cachedMaterial != null)
            {
                Destroy(_cachedMaterial);
            }
        }
    }
}
