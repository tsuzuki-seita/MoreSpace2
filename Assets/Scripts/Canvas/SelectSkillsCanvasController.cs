using MoreSpace.Presentation;
using UnityEngine;

namespace MoreSpace.SelectSkills
{
    public class SelectSkillsCanvasController : MonoBehaviour
    {
        [SerializeField] private SelectSkillsCanvasView view;

        void Start()
        {
            view.OnInputGoMatching += () => IngameSceneManager.Instance.ChangeScene(InGameState.Matching);
            view.OnInputBack += () => IngameSceneManager.Instance.ChangeScene(InGameState.Title);
            view.Initialize();
        }
    }
}