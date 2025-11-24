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
            view.OnInputGoSelectSkills += () =>
            {
                IngameSceneManager.Instance.ChangeScene(InGameState.SelectSkills);
                SoundManager.Instance.PlaySE(SoundManager.SEData.SETYPE.Button);
            };
            view.Initialize();
            SoundManager.Instance.PlayBGM(SoundManager.BGMData.BGMTYPE.Title);
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