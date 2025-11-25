using UnityEngine;
using MoreSpace.InGame.Player; // PlayerHpを参照するために追加

namespace MoreSpace.InGame.Weapons
{
    [DisallowMultipleComponent]
    public sealed class Unbreakable : Active
    {
        [Header("Unbreakable Settings")]
        [Tooltip("発動時に有効化するオブジェクト（バリアのエフェクト等）")]
        [SerializeField] private GameObject effectObject;

        // PlayerHpへの参照を保持
        private PlayerHp _playerHp;
        
        private Material _targetMaterial;
        private int _isUnbreakablePropID;

        private void Awake()
        {
            // 親オブジェクト等から PlayerHp コンポーネントを探して取得
            _playerHp = GetComponentInParent<PlayerHp>();
            if (_playerHp == null)
            {
                Debug.LogError($"[Unbreakable] PlayerHp component not found in parents of {name}!");
            }

            _isUnbreakablePropID = Shader.PropertyToID("_isUnbreakable"); 

            // まずは直下の子を検索
            var t = transform.Find("UnbreakableEffect");

            // 見つからなければ、孫以降も含めて検索（非アクティブも含む）
            if (t == null)
            {
                foreach (var child in GetComponentsInChildren<Transform>(true))
                {
                    if (child.name == "UnbreakableEffect")
                    {
                        t = child;
                        break;
                    }
                }
            }

            if (t != null)
            {
                effectObject = t.gameObject;
            }
            else
            {
                Debug.LogWarning(
                    "[Unbreakable] 子オブジェクトから 'UnbreakableEffect' を見つけられませんでした。",
                    this
                );
            }

            FindTargetMaterial();
        }

        protected override void OnActivateStart()
        {
            SoundManager.Instance.PlaySE(SoundManager.SEData.SETYPE.Stealth);
            // --- HPの無敵フラグをON ---
            if (_playerHp != null) _playerHp.IsUnbreakable = true;
            
            // エフェクト有効化
            if (effectObject != null) effectObject.SetActive(true);

            // マテリアル変更
            if (_targetMaterial != null)
                _targetMaterial.SetFloat(_isUnbreakablePropID, 1.0f);
        }

        protected override void OnActivateStop()
        {
            // --- HPの無敵フラグをOFF ---
            if (_playerHp != null) _playerHp.IsUnbreakable = false;

            // エフェクト無効化
            if (effectObject != null) effectObject.SetActive(false);

            // マテリアル戻す
            if (_targetMaterial != null)
                _targetMaterial.SetFloat(_isUnbreakablePropID, 0.0f);
        }

        private void FindTargetMaterial()
        {
            var renderers = GetComponentsInChildren<Renderer>(true);
            foreach (var r in renderers)
            {
                var mats = r.materials; 
                foreach (var m in mats)
                {
                    if (m != null && m.name.Contains("UnbreakablePlayer"))
                    {
                        _targetMaterial = m;
                        _targetMaterial.SetFloat(_isUnbreakablePropID, 0.0f);
                        return;
                    }
                }
            }
        }

        public override void OnFire()
        {
        }

        public override void OnFireUp()
        {
        }
    }
}