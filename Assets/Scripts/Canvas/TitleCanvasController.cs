using MoreSpace.Presentation;
using UnityEngine;

namespace MoreSpace.Title
{
    public class TitleCanvasController : MonoBehaviour
    {
        [SerializeField] private TitleCanvasView view;

        void Start()
        {
            view.OnInputGoSelectSkills += () => IngameSceneManager.Instance.ChangeScene(InGameState.SelectSkills);
            view.Initialize();
        }
    }
}