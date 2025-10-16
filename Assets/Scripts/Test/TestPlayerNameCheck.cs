using MoreSpace.Presentation;
using UnityEngine;

public class TestPlayerNameCheck : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var saved = IngameSceneManager.Instance.LoadUserName();
        Debug.Log($"SavedName={saved}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
