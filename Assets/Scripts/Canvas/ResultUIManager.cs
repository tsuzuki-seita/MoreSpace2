using MoreSpace.Domain;
using MoreSpace.InGame.Master;
using MoreSpace.Presentation;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace MoreSpace.Result
{
    public class ResultUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] uis = new GameObject[4];
        [SerializeField] private Button backButton;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            foreach (var text in uis)
                text.SetActive(false);
            if (IngameSceneManager.Instance != null
                && IngameSceneManager.Instance.TryConsume<ResultArgs>(out var args))
            {
                uis[(int)args.Result].SetActive(true);
                if ((int)args.Result == 0)
                    SoundManager.Instance.PlayBGM(SoundManager.BGMData.BGMTYPE.Winner);
                else if ((int)args.Result == 1)
                    SoundManager.Instance.PlayBGM(SoundManager.BGMData.BGMTYPE.Loser);
            }

            backButton.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySE(SoundManager.SEData.SETYPE.Button);
                IngameSceneManager.Instance.ChangeScene(InGameState.Title);
            });
            
        }
    }
}