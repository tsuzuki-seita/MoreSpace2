using System;
using MoreSpace.InGame.Player;
using Photon.Pun;
using UnityEngine;

public class PlayerMaker : MonoBehaviour
{
    [SerializeField] private GameObject[] playersPrefab = new GameObject[2];
    [SerializeField] private Vector3[] startPosition = new Vector3[2];
    [SerializeField] private Transform[] planets;
    public PlayerModel model { get; private set; }

    private void Awake()
    {
        model = new PlayerModel()
        {
            Planets = planets
        };
    }

    void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;
        var playerIndex = PhotonNetwork.IsMasterClient ? 0 : 1;
        MakePlayer(playerIndex);
    }

    void MakePlayer(int index)
    {
        var player = PhotonNetwork.Instantiate(playersPrefab[index].name,startPosition[index],Quaternion.identity);
        SkillController.Instance.SetPlayer(player);
    }
}
