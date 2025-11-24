using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MoreSpace.Title
{
    public class TitleCanvasView : MonoBehaviour
    {
        public UnityAction OnInputGoSelectSkills;
        public UnityAction<string> OnNameInput;
        public UnityAction OnInputTutorial;
        public UnityAction OnInputSetting;
        public UnityAction OnInputCloseSetting;
        [SerializeField] private Button goSelectSkills;
        [SerializeField] private InputField nickName;
        [SerializeField] private Button goTutorial;
        [SerializeField] private Button goSetting;
        [SerializeField] private Button settingsBackgroundButton;
        [SerializeField] public GameObject SettingsPanel;
        

        public void Initialize()
        {
            goSelectSkills.onClick.AddListener(OnInputGoSelectSkills);
            nickName.onValueChanged.AddListener(OnNameInput);
            goTutorial.onClick.AddListener(OnInputTutorial);
            goSetting.onClick.AddListener(OnInputSetting);
            SettingsPanel.SetActive(false);
            settingsBackgroundButton.onClick.AddListener(OnInputCloseSetting);
        }
    }
}