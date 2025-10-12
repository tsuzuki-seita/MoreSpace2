using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RoomViewer : MonoBehaviour
{
    public UnityAction OnInputDisconnectButton;
    public UnityAction OnChangeState;
    
    [SerializeField] private Text[] playerName = new Text[2];
    [SerializeField] private Text[] playerState = new Text[2];
    [SerializeField] private Button localStateChangeButton;
    [SerializeField] private Button disconnectButton;

    public void Initialize()
    {
        localStateChangeButton.onClick.AddListener(OnChangeState);
        disconnectButton.onClick.AddListener(OnInputDisconnectButton);
    }

    public void UpdateLocalPlayer(string localName, bool ready)
    {
        playerName[0].text = localName;
        playerState[0].text = ReadyToString(ready);
    }

    public void UpdateRemotePlayer(string remoteName, bool ready)
    {
        playerName[1].text = remoteName;
        playerState[1].text = ReadyToString(ready);
    }

    string ReadyToString(bool b)
    {
        return b ? "Ready!" : "Ready?";
    }
}
