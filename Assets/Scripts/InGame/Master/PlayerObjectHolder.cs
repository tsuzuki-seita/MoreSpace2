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
        public readonly Dictionary<Photon.Realtime.Player,PhotonView> player = new ();
        
        public void SetPlayer(PhotonView p)
        {
            player.Add(p.Owner,p);
            OnAddPlayer?.Invoke(p);
        }
    }
}