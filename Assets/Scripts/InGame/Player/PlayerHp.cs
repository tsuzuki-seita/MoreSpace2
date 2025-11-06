using UnityEngine;
using MoreSpace.InGame.Master;
using Photon.Pun;

namespace MoreSpace.InGame.Player
{
    public class PlayerHp : HealthBase
    {
        public override void Die()
        {
            Destroy(gameObject);
        }
    }
}

