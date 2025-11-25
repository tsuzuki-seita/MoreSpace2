using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace MoreSpace.Tutorial
{
    public class TutorialPopupUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject root;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Text titleText;
        [SerializeField] private Text messageText;

        [Header("Settings")]
        [SerializeField] private float fadeDuration = 0.3f;
        [SerializeField] private float maxNoiseAmount = 0.5f;
        
        [Header("Shader Settings")]
        [SerializeField] private Shader noiseShader; // Inspectorで割り当てる
        [SerializeField] private Image m_graphic;
        private Material m_mat;// Shader Property IDs (高速化のためID化)
        private static readonly int HorizonValueId = Shader.PropertyToID("_HorizonValue");
        private static readonly int SeedId = Shader.PropertyToID("_Seed");

        private Action _onClosed;
        private CancellationTokenSource _cts;

        private void Awake()
        {
            if (canvasGroup == null && root != null)
            {
                canvasGroup = root.GetComponent<CanvasGroup>();
                if (canvasGroup == null) canvasGroup = root.AddComponent<CanvasGroup>();
            }

            // マテリアルを複製して適用（他のUIに影響させないため）
            if (noiseShader != null && m_graphic != null)
            {
                m_mat = new Material(noiseShader);
                m_graphic.material = m_mat;
                m_mat.SetFloat(HorizonValueId, 0f);
            }
            else
            {
                Debug.LogWarning("UI Noise Shader or Target Graphic not found!");
            }
        }

        private void Update()
        {
            if(root.activeSelf && Input.GetKeyDown(KeyCode.Space))
                Close();
            // ポップアップが表示されている間、砂嵐のようにノイズを動かす
            if (root != null && root.activeSelf && m_mat != null)
            {
                m_mat.SetInt(SeedId, Time.frameCount);
            }
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            if (m_mat != null)
            {
                Destroy(m_mat);
                m_graphic.material = null;
            }
        }

        public void Show(string title, string message, Action onClosed)
        {
            SoundManager.Instance.PlaySE(SoundManager.SEData.SETYPE.SelectMove);
            _onClosed = onClosed;
            if (titleText != null) titleText.text = title;
            if (messageText != null) messageText.text = message;
            
            ChangeVisualize(true);
        }

        private void Close()
        {
            SoundManager.Instance.PlaySE(SoundManager.SEData.SETYPE.Button);
            // コールバック実行
            _onClosed?.Invoke();
            _onClosed = null;

            // 非表示アニメーション開始
            ChangeVisualize(false);
        }

        void ChangeVisualize(bool toVisualize)
        {
            // 実行中のアニメーションがあればキャンセル
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            // Fire-and-Forgetでタスク実行
            ChangeVisualizeTask(toVisualize, _cts.Token).Forget();
        }

        async UniTaskVoid ChangeVisualizeTask(bool toVisualize, CancellationToken token)
        {
            float endAlpha = toVisualize ? 1f : 0f;
            float startNoise = toVisualize ? maxNoiseAmount : 0f;
            float endNoise = toVisualize ? 0f : maxNoiseAmount;
            
            if (toVisualize)
            {
                if (root != null)
                {
                    canvasGroup.alpha = 0f;
                }
                // 表示開始直後、一瞬だけノイズ全開にする
                if (m_mat != null) m_mat.SetFloat(HorizonValueId, startNoise);
            }

            try
            {
                var fadeTask = canvasGroup.DOFade(endAlpha, fadeDuration)
                    .SetUpdate(true) // TimeScale=0でも動く
                    .SetLink(gameObject)
                    .ToUniTask(cancellationToken: token);

                UniTask shaderTask = UniTask.CompletedTask;
                if (m_mat != null)
                {
                    shaderTask = m_mat.DOFloat(endNoise, HorizonValueId, fadeDuration/2)
                        .SetUpdate(true)
                        .SetLink(gameObject)
                        .ToUniTask(cancellationToken: token);
                }
                Debug.Log($"Fade開始{fadeTask.Status}/{shaderTask.Status}");
                await UniTask.WhenAll(fadeTask, shaderTask);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            if (toVisualize)
            {
                if (m_mat != null) m_mat.SetFloat(HorizonValueId, 0f);
            }
        }
    }
}