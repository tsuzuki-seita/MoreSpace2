using System;
using MoreSpace.Domain;
using MoreSpace.InGame;
using MoreSpace.InGame.Player;
using MoreSpace.Presentation;
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

    [SerializeField] private StartGameWithCountDown countdown;
    
    private PlayerModel _model;

    private void Start()
    {
        SkillController.Instance.SetPlayer(photonView);
        
        if (photonView.IsMine)
        {
            _model = FindAnyObjectByType<PlayerMaker>().model;
            FindAnyObjectByType<LookUiToCamera>().AssertCamera(playerCamera.transform);
            AssertData(_model);
            SetUIs(true);
        }
        else
        {
            DisableComponents();
            SetUIs(false);
        }
        
        countdown.OnEndPrepare();
    }

    void AssertData(PlayerModel model)
    {
        IngameSceneManager.Instance.TryConsume<StartIngameArgs>(out var args);
        SkillController.Instance.SetSelectedSkills(args?.SelectedSkills);
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