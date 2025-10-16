using MoreSpace.Domain;
using MoreSpace.Presentation;
using UnityEngine;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] private Button button;

    void Start()
    {
        button.onClick.AddListener(OnClickStartButton);
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void OnClickStartButton()
    {
        var loadout = new SkillLoadout(new("Laser"), new("Homing"), new("Stealth"));
        IngameSceneManager.Instance.ChangeScene(InGameState.Ingame);
        IngameSceneManager.Instance.ChangeScene(InGameState.GameOver, new IngameArgs(loadout));
        IngameSceneManager.Instance.ChangeScene(InGameState.SampleScene, new UpdateUserName("contena"));
    }
}
