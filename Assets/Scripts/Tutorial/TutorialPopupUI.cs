using System;
using UnityEngine;

namespace MoreSpace.Tutorial
{
    public class TutorialPopupUI : MonoBehaviour
    {
        [SerializeField] private GameObject root;   // ポップアップ全体の親
        [SerializeField] private TMPro.TextMeshProUGUI messageText;

        private Action _onClosed;

        private void Awake()
        {
            if (root != null)
            {
                root.SetActive(false);
            }
        }

        public void Show(string message, Action onClosed)
        {
            _onClosed = onClosed;
            if (messageText != null)
            {
                messageText.text = message;
            }

            if (root != null)
            {
                root.SetActive(true);
            }
        }

        public void Close()
        {
            if (root != null)
            {
                root.SetActive(false);
            }

            _onClosed?.Invoke();
            _onClosed = null;
        }
    }
}
