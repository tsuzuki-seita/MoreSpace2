using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace MoreSpace.InGame.Player
{
    public class PlayerObjectHolder : SingletonMonoBehaviourPunCallbacks<PlayerObjectHolder>
    {
        public UnityAction<PhotonView> OnAddPlayer;
        public readonly List<PhotonView> player = new List<PhotonView>();
        
        public void SetPlayer(PhotonView p)
        {
            player.Add(p);
            OnAddPlayer?.Invoke(p);
        }
    }
}