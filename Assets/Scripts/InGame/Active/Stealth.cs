using System.Collections.Generic;
using UnityEngine;

namespace MoreSpace.InGame.Weapons
{
    [DisallowMultipleComponent]
    public sealed class Stealth : Active
    {
        [Header("Visual")]
        [Range(0f, 1f)]
        public float StealthAlpha = 0.15f;

        private readonly Dictionary<Material, Color> _originalColors = new();
        private Renderer[] _renderers;

        private static readonly int ColorProp     = Shader.PropertyToID("_Color");
        private static readonly int BaseColorProp = Shader.PropertyToID("_BaseColor");

        private void Awake()
        {
            _renderers = GetComponentsInChildren<Renderer>(true);
            CacheOriginalColors();
        }

        protected override void OnActivateStart()
        {
            SetAlphaOnAll(StealthAlpha);
            // 必要なら当たり判定・UI・ターゲット不可などをここで制御
        }

        protected override void OnActivateStop()
        {
            RestoreOriginalColors();
        }

        private void CacheOriginalColors()
        {
            _originalColors.Clear();
            foreach (var r in _renderers)
            {
                var mats = r.materials; // インスタンス化済み：共有アセットを汚さない
                foreach (var m in mats)
                {
                    if (m == null) continue;

                    if (m.HasProperty(BaseColorProp))
                        _originalColors[m] = m.GetColor(BaseColorProp);
                    else if (m.HasProperty(ColorProp))
                        _originalColors[m] = m.GetColor(ColorProp);
                }
            }
        }

        private void SetAlphaOnAll(float a)
        {
            foreach (var kv in _originalColors)
            {
                var mat = kv.Key;
                var col = kv.Value;
                col.a = a;

                if (mat.HasProperty(BaseColorProp)) mat.SetColor(BaseColorProp, col);
                else if (mat.HasProperty(ColorProp)) mat.SetColor(ColorProp, col);
            }
        }

        private void RestoreOriginalColors()
        {
            foreach (var kv in _originalColors)
            {
                var mat = kv.Key;
                var col = kv.Value;

                if (mat.HasProperty(BaseColorProp)) mat.SetColor(BaseColorProp, col);
                else if (mat.HasProperty(ColorProp)) mat.SetColor(ColorProp, col);
            }
        }
    }
}
