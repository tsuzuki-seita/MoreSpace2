using Photon.Pun;
using UnityEngine;

namespace MoreSpace.InGame.Player
{
    public class VisualizeWireOnAttackCrystal : MonoBehaviourPun
    {
        [SerializeField] private MeshRenderer wireRenderer;
        [SerializeField] private float visualizeTime = 2.0f; // 表示時間

        // シェーダーのプロパティID
        private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");
        private static readonly int XRayColorID = Shader.PropertyToID("_XRayColor");

        private void Start()
        {
            if (wireRenderer == null) wireRenderer = GetComponent<MeshRenderer>();

            // 【重要】確実に色が見えるように、Startで色をセットしてしまう
            // DebugXRayAlways と同じ設定値にします
            Material mat = wireRenderer.material;
            mat.SetColor(BaseColorID, new Color(1f, 0f, 0f, 0.3f)); // 赤 (手前)
            mat.SetColor(XRayColorID, new Color(0f, 1f, 0f, 1f));   // 緑 (奥/XRay)

            // 初期状態は非表示
            wireRenderer.enabled = false;
        }

        /// <summary>
        /// 外部から呼ばれるトリガー
        /// </summary>
        public void StartVisualize()
        {
            // 全員（自分含む）に対して「表示しろ」と命令を送る
            photonView.RPC(nameof(RpcShowWireframe), RpcTarget.All);
        }

        [PunRPC]
        private void RpcShowWireframe()
        {
            // 誰であろうと、命令が来たら表示する
            wireRenderer.enabled = true;

            // 以前の実行待ちがあればキャンセルして、新しくタイマーセット
            CancelInvoke(nameof(HideWireframe));
            Invoke(nameof(HideWireframe), visualizeTime);
        }

        private void HideWireframe()
        {
            wireRenderer.enabled = false;
        }
    }
}