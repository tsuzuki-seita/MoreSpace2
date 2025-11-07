using System;
using MoreSpace.InGame;
using MoreSpace.InGame.Player;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class SetDataToPlayer : MonoBehaviourPunCallbacks
{
    [SerializeField] private PlayerMover mover;
    [SerializeField] private PlayerRotater rotater;
    [SerializeField] private PlanetGravity gravity;
    [SerializeField] private GameObject worldCanvas;
    [SerializeField] private GameObject localCanvas;
    [SerializeField] private GameObject playerCamera;
    private PlayerModel _model;

    private void Start()
    {
        if (photonView.IsMine)
        {
            _model = FindAnyObjectByType<PlayerMaker>().model;
            FindAnyObjectByType<LookUiToCamera>().AssertUI(playerCamera.transform);
            AssertData(_model);
            SetUIs(true);
        }
        else
        {
            DisableComponents();
            SetUIs(false);
        }
    }

    void AssertData(PlayerModel model)
    {
        gravity.SetPlanets(model.Planets);
    }

    void DisableComponents()
    {
        mover.enabled = false;
        rotater.enabled = false;
        gravity.enabled = false;
        playerCamera.SetActive(false);
    }

    void SetUIs(bool isLocalPlayer)
    {
        localCanvas.SetActive(isLocalPlayer);
        worldCanvas.SetActive(!isLocalPlayer);
    }
}

public class PlayerModel
{
    public Transform[] Planets;
}