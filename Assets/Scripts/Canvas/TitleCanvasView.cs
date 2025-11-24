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
        [SerializeField] private Slider _masterVolumeSlider;
        [SerializeField] private Slider _bgmVolumeSlider;
        [SerializeField] private Slider _seVolumeSlider;
        

        public void Initialize()
        {
            goSelectSkills.onClick.AddListener(OnInputGoSelectSkills);
            nickName.onValueChanged.AddListener(OnNameInput);
            goTutorial.onClick.AddListener(OnInputTutorial);
            goSetting.onClick.AddListener(OnInputSetting);
            SetInitialSliderValues();
            RegisterSliderListeners();
            SettingsPanel.SetActive(false);
            settingsBackgroundButton.onClick.AddListener(OnInputCloseSetting);
        }
        
        private void SetInitialSliderValues()
        {
            _masterVolumeSlider.value = SoundManager.Instance.masterVolume;
            _bgmVolumeSlider.value = SoundManager.Instance.bgmMasterVolume;
            _seVolumeSlider.value = SoundManager.Instance.seMasterVolume;
        }
        private void RegisterSliderListeners()
        {
            // AddListenerで、値変更時に対応する On...Changed メソッドを呼び出すように登録
            _masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            _bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
            _seVolumeSlider.onValueChanged.AddListener(OnSEVolumeChanged);
        }
        private void OnMasterVolumeChanged(float newValue)
        {
            SoundManager.Instance.SetMasterVolume(newValue);
        }
    
        private void OnBGMVolumeChanged(float newValue)
        {
            SoundManager.Instance.SetBGMVolume(newValue);
        }

        private void OnSEVolumeChanged(float newValue)
        {
            SoundManager.Instance.SetSEVolume(newValue);
        }
    }
}