using System.Collections.Generic;
using MoreSpace.InGame;
using MoreSpace.InGame.Master;
using Photon.Pun;
using UnityEngine;

public class PlayerMaker : MonoBehaviour
{
    [SerializeField] private GameObject[] playersPrefab = new GameObject[2];
    [SerializeField] private Vector3[] startPosition = new Vector3[2];
    [SerializeField] private Vector3[] startRotation = new Vector3[2];
    [SerializeField] private Transform[] planets;
    [SerializeField] private bool isOffline = false;
    public PlayerModel model { get; private set; }

    private void Awake()
    {
        if (isOffline)
        {
            PhotonNetwork.IsMessageQueueRunning = true;
            PhotonNetwork.OfflineMode = true;
            PhotonNetwork.CreateRoom(null);
        }
        model = new PlayerModel()
        {
            Planets = planets
        };
        CheckDestroyOnMasterClient.Instance.ResetData();
        DamageableHolder.Holders = new Dictionary<int, IDamageable>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;
        var playerIndex = PhotonNetwork.IsMasterClient ? 0 : 1;
        MakePlayer(playerIndex);
        SoundManager.Instance.PlayBGM(SoundManager.BGMData.BGMTYPE.InGame);
    }

    void MakePlayer(int index)
    {
        PhotonNetwork.Instantiate(playersPrefab[index].name, startPosition[index], Quaternion.Euler(startRotation[index]));
    }
}
