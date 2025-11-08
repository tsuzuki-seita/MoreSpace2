using UnityEngine;

namespace MoreSpace.InGame.Weapons
{
    [DisallowMultipleComponent]
    public sealed class Unbreakable : Active
    {
        // 見た目用のVFXなどがあればここで参照
        // [SerializeField] private GameObject shieldVfx;

        protected override void OnActivateStart()
        {
            // if (shieldVfx) shieldVfx.SetActive(true);
            // 他、サウンドなど
        }

        protected override void OnActivateStop()
        {
            // if (shieldVfx) shieldVfx.SetActive(false);
        }
    }
}
