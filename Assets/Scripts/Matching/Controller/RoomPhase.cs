using System;
using System.Linq;
using MoreSpace.Domain;
using MoreSpace.InGame.Master;
using MoreSpace.InGame.Player;
using MoreSpace.Presentation;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomPhase : IPhase
{
    [SerializeField] private RoomViewer viewer;
    
    protected override void OnInitializePhase()
    {
        viewer.OnInputDisconnectButton += Disconnect;
        viewer.OnChangeState += () =>
        {
            SoundManager.Instance.PlaySE(SoundManager.SEData.SETYPE.Button);
            photonView.RPC(nameof(ChangeState), RpcTarget.All, GetPlayerIndex(PhotonNetwork.LocalPlayer));
        };
        ResetModelData();
        viewer.Initialize();
        
        SetLocalPlayerToView();
        if(PhotonNetwork.PlayerList.Length == 2)
            viewer.UpdateRemotePlayer(PhotonNetwork.PlayerListOthers[0].NickName,false);
        else
            SetRemotePlayerWaiting();
    }

    void ResetModelData()
    {
        _router.Model.IsPlayerReady = new bool[2];
    }

    [PunRPC]
    void ChangeState(int index, PhotonMessageInfo info)
    {
        _router.Model.IsPlayerReady[index] = !_router.Model.IsPlayerReady[index];
        CheckAllPlayerReady();
        if(info.Sender.Equals(PhotonNetwork.LocalPlayer))
            SetLocalPlayerToView();
        else
            viewer.UpdateRemotePlayer(info.Sender.NickName,_router.Model.IsPlayerReady[index]);
    }

    void CheckAllPlayerReady()
    {
        if (_router.Model.IsPlayerReady.All(b => b))
        {
            DamageableHolder.Holders.Clear();
            var skillSets = GetSkillSet();
            PhotonNetwork.IsMessageQueueRunning = false;
            IngameSceneManager.Instance.ChangeScene(InGameState.Ingame, skillSets);
        }
    }

    StartIngameArgs GetSkillSet()
    {
        if (IngameSceneManager.Instance != null
            && IngameSceneManager.Instance.TryConsume<StartIngameArgs>(out var args))
        {
            Debug.Log("PlayerMaker: StartIngameArgs read (sticky) and passed to SkillController.");
            return args;
        }
        else
        {
            Debug.LogWarning("PlayerMaker: StartIngameArgs が見つかりません（Sticky モードでも未セット）");
            return null;
        }
    }
    
    void Disconnect()
    {
        SoundManager.Instance.PlaySE(SoundManager.SEData.SETYPE.Button);
        isBack = true;
        PhotonNetwork.Disconnect();
        IngameSceneManager.Instance.ChangeScene(InGameState.Title);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        ResetModelData();
        SetLocalPlayerToView();
        viewer.UpdateRemotePlayer(newPlayer.NickName,false);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Disconnect();
    }

    int GetPlayerIndex(Player target)
    {
        return Array.IndexOf(PhotonNetwork.PlayerList, target);
    }

    void SetLocalPlayerToView()
    {
        viewer.UpdateLocalPlayer(PhotonNetwork.LocalPlayer.NickName,
            _router.Model.IsPlayerReady[GetPlayerIndex(PhotonNetwork.LocalPlayer)]);
    }

    void SetRemotePlayerWaiting()
    {
        viewer.UpdateRemotePlayer("Waiting",false);
    }
}
