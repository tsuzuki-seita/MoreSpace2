using System;
using MoreSpace.Domain;
using MoreSpace.InGame.Player;
using MoreSpace.Presentation;
using Photon.Pun;
using UnityEngine;

public class PlayerMaker : MonoBehaviour
{
    [SerializeField] private GameObject[] playersPrefab = new GameObject[2];
    [SerializeField] private Vector3[] startPosition = new Vector3[2];
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
    }

    void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;
        var playerIndex = PhotonNetwork.IsMasterClient ? 0 : 1;
        MakePlayer(playerIndex);
    }

    void MakePlayer(int index)
    {
        var player = PhotonNetwork.Instantiate(playersPrefab[index].name, startPosition[index], Quaternion.identity);

        // SkillController に owner をセット
        SkillController.Instance.SetPlayer(player);

        if (IngameSceneManager.Instance != null
            && IngameSceneManager.Instance.TryConsume<StartIngameArgs>(out var args))
        {
            SkillController.Instance.SetSelectedSkills(args.SelectedSkills);
            Debug.Log("PlayerMaker: StartIngameArgs read (sticky) and passed to SkillController.");
        }
        else
        {
            Debug.LogWarning("PlayerMaker: StartIngameArgs が見つかりません（Sticky モードでも未セット）");
        }
    }
}
