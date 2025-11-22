using Photon.Pun;
using UnityEngine;

namespace MoreSpace.InGame
{
    public class SetCrystal : MonoBehaviour
    {
        void Awake()
        {
            this.transform.rotation = Random.rotationUniform;
        }
    }
}
