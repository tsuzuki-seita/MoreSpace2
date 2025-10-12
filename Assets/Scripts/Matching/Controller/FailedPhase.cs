using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FailedPhase : IPhase
{
    [SerializeField] private Text causeText;
    [SerializeField] private Button backButton;
    
    protected override void OnInitializePhase()
    {
        causeText.text = SetErrorMessage( _router.Model.DisconnectCause);
        backButton.onClick.AddListener(Back);
    }

    private void Back()
    {
        SceneManager.LoadScene("Title");
    }

    private string SetErrorMessage(DisconnectCause cause)
    {
        switch (cause)
        {
            case DisconnectCause.ClientTimeout:
                return "タイムアウトしました。\n通信状況を確認してください。";
            case DisconnectCause.DisconnectByClientLogic:
                return "切断しました。";
            case DisconnectCause.ExceptionOnConnect:
                return "接続に失敗しました。\n通信状況を確認してください。";
            default:
                return "切断されました。";
        }
    }
}
