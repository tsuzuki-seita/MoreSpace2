using MoreSpace.Presentation;
using Photon.Pun;
using UnityEngine;

namespace MoreSpace.Title
{
    public class TitleCanvasController : MonoBehaviour
    {
        [SerializeField] private TitleCanvasView view;
        [SerializeField] private string baseName = "User";
        
        void Start()
        {
            OnEdit(baseName);
            view.OnNameInput += OnEdit;
            view.OnInputSetting += OnSettingsButton;
            view.OnInputCloseSetting += CloseSettingsPanel;
            view.OnInputTutorial += () => IngameSceneManager.Instance.ChangeScene(InGameState.Tutorial);
            view.OnInputGoSelectSkills += () =>
            {
                SoundManager.Instance.PlaySE(SoundManager.SEData.SETYPE.Button);
                IngameSceneManager.Instance.ChangeScene(InGameState.SelectSkills);
            };
            view.Initialize();
            SoundManager.Instance.PlayBGM(SoundManager.BGMData.BGMTYPE.Title);

            PhotonNetwork.OfflineMode = false;
        }

        void OnEdit(string target)
        {
            PhotonNetwork.NickName = target;
        }
        void OnSettingsButton()
        {
            view.SettingsPanel.SetActive(true);
            SoundManager.Instance.PlaySE(SoundManager.SEData.SETYPE.Button);
        }
        void CloseSettingsPanel()
        {
            if (view.SettingsPanel.activeSelf)
            {
                view.SettingsPanel.SetActive(false);
                SoundManager.Instance.PlaySE(SoundManager.SEData.SETYPE.Button); // 閉じるSEがあれば再生
            }
        }
    }
}