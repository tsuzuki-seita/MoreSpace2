using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MoreSpace.SelectSkills
{
    public class SelectSkillsCanvasView : MonoBehaviour
    {
        public UnityAction OnInputGoMatching;
        public UnityAction OnInputBack;
        [SerializeField] private Button goMatching;
        [SerializeField] private Button goBack;

        public void Initialize()
        {
            goMatching.onClick.AddListener(OnInputGoMatching);
            goBack.onClick.AddListener(OnInputBack);
        }
    }
}