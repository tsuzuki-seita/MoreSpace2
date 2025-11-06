using Photon.Pun;
using UnityEngine;

namespace MoreSpace.InGame
{
    public class SetCrystal : MonoBehaviour
    {
        [SerializeField] private GameObject crystalPrefab;
        [SerializeField] private float makeRange;

        void Start()
        {
            if(!PhotonNetwork.IsMasterClient) return;
            Vector3 makeVector = Random.onUnitSphere.normalized;
            var crystal = PhotonNetwork.Instantiate(crystalPrefab.name,Vector3.zero, Quaternion.identity);
            crystal.transform.parent = this.transform;
            crystal.transform.localPosition = makeVector * makeRange;
            crystal.transform.LookAt(this.transform);
        }
    }
}
