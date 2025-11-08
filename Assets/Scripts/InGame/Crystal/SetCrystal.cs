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
            var scaleOffset = this.transform.localScale.x;
            crystal.transform.localScale = Vector3.one * 75 * scaleOffset;
            crystal.transform.localPosition = this.transform.position + makeVector * makeRange * scaleOffset;
            crystal.transform.LookAt(this.transform);
        }
    }
}
