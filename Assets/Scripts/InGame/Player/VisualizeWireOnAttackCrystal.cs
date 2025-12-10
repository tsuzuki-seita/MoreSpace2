using Photon.Pun;
using UnityEngine;
using UnityEngine.Rendering;

namespace MoreSpace.InGame.Player
{
    public class VisualizeWireOnAttackCrystal : MonoBehaviourPun
    {
        [SerializeField] private MeshRenderer wireRenderer;
        [SerializeField] private float visualizeTime = 2.0f; // 表示時間

        // シェーダーのプロパティID
        private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");
        private static readonly int ZTest = Shader.PropertyToID("_ZTest");
        private static readonly int ZWrite = Shader.PropertyToID("_ZWrite");

        private void Start()
        {
            if (wireRenderer == null) wireRenderer = GetComponent<MeshRenderer>();
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
        private void RpcShowWireframe(PhotonMessageInfo info)
        {
            if(info.Sender.IsLocal) return;
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