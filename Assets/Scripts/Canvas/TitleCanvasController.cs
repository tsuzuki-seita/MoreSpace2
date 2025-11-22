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
            view.OnInputGoSelectSkills += () => IngameSceneManager.Instance.ChangeScene(InGameState.SelectSkills);
            view.Initialize();
            SoundManager.Instance.PlayBGM(SoundManager.BGMData.BGMTYPE.Title);
        }

        void OnEdit(string target)
        {
            PhotonNetwork.NickName = target;
        }
    }
}