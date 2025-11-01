using System;
using Photon.Pun;
using UnityEngine;

public class PlayerMaker : MonoBehaviour
{
    [SerializeField] private GameObject[] playersPrefab = new GameObject[2];
    [SerializeField] private Vector3[] startPosition = new Vector3[2];
    [SerializeField] private Transform[] planets;
    
    void Start()
    {
        var playerIndex = Array.IndexOf(PhotonNetwork.PlayerList, PhotonNetwork.LocalPlayer);
        MakePlayer(playerIndex);
    }

    void MakePlayer(int index)
    {
        var target = PhotonNetwork.Instantiate(playersPrefab[index].name,startPosition[index],Quaternion.identity);
        PlayerModel model = new PlayerModel()
        {
            Planets = planets
        };
        target.GetComponent<SetDataToPlayer>().Initialize(model);
    }
}
