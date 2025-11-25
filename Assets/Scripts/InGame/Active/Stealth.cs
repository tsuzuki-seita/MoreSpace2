using UnityEngine;
using Photon.Pun; // 自分視点/相手視点の判定用

namespace MoreSpace.InGame.Weapons
{
    [DisallowMultipleComponent]
    public sealed class Stealth : Active
    {
        [Header("Stealth Settings")]
        [Tooltip("自分から見たときの透明度 (0:完全透明 1:元のまま)")]
        [Range(0f, 1f)]
        public float selfAlpha = 0.1f;

        [Header("Shader Property")]
        [Tooltip("UnbreakablePlayer の SurfaceInputs.Alpha に対応するプロパティ名")]
        [SerializeField] private string alphaPropertyName = "_Alpha";

        private Material _targetMaterial;
        private int _alphaPropID;
        private PhotonView _view;

        private void Awake()
        {
            _view = GetComponentInParent<PhotonView>();
            _alphaPropID = Shader.PropertyToID(alphaPropertyName);

            FindTargetMaterial();
        }

        /// <summary>
        /// Unbreakable と同じ方法で UnbreakablePlayer マテリアルを取得
        /// </summary>
        private void FindTargetMaterial()
        {
            var renderers = GetComponentsInChildren<Renderer>(true);
            foreach (var r in renderers)
            {
                if (r == null) continue;

                var mats = r.materials; // インスタンス化されたマテリアル
                foreach (var m in mats)
                {
                    if (m == null) continue;

                    // UnbreakablePlayer を使っているマテリアルだけ対象
                    if (!m.name.Contains("UnbreakablePlayer"))
                        continue;

                    // Alpha プロパティを持っているかチェック
                    if (!m.HasProperty(_alphaPropID))
                        continue;

                    _targetMaterial = m;

                    // 念のため初期状態は 1 にしておく（元の見た目）
                    _targetMaterial.SetFloat(_alphaPropID, 1.0f);

                    Debug.Log($"[Stealth] Target material found: {_targetMaterial.name}", this);
                    return;
                }
            }

            if (_targetMaterial == null)
            {
                Debug.LogWarning("[Stealth] UnbreakablePlayer マテリアルが見つかりませんでした。", this);
            }
        }

        protected override void OnActivateStart()
        {
            if (_targetMaterial == null) return;

            SoundManager.Instance.PlaySE(SoundManager.SEData.SETYPE.Stealth);

            // 自分から見えるときは少し透明、他人からは完全透明
            if (_view != null && _view.IsMine)
            {
                // 自分視点：SurfaceInputs.Alpha を selfAlpha に
                _targetMaterial.SetFloat(_alphaPropID, selfAlpha);
            }
            else
            {
                // 相手視点：完全透明
                _targetMaterial.SetFloat(_alphaPropID, 0.0f);
            }
        }

        protected override void OnActivateStop()
        {
            if (_targetMaterial == null) return;

            // Alpha を 1 に戻せば元の見た目に戻る
            _targetMaterial.SetFloat(_alphaPropID, 1.0f);
        }

        public override void OnFire()
        {
        }

        public override void OnFireUp()
        {
        }
    }
}
