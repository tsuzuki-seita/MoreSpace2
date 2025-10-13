using System;
using Photon.Pun;
using UnityEngine;

public class SetDataToPlayer : MonoBehaviourPunCallbacks
{
    [SerializeField] private PlayerMover mover;
    [SerializeField] private PlayerRotater rotater;
    [SerializeField] private PlanetGravity gravity;
    [SerializeField] private GameObject playerCamera;
    private PlayerModel _model;

    private void Start()
    {
        if (photonView.IsMine)
            AssertData(_model);
        else
            DisableComponents();
    }

    public void Initialize(PlayerModel model)
    {
        _model = model;
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
}

public class PlayerModel
{
    public Transform[] Planets;
}