using UnityEngine;

namespace MoreSpace.InGame
{
    public class SetCrystal : MonoBehaviour
    {
        [SerializeField] private GameObject crystalPrefab;
        [SerializeField] private float makeRange;

        void Start()
        {
            Vector3 makeVector = Random.onUnitSphere.normalized;
            var crystal = Instantiate(crystalPrefab, this.transform);
            crystal.transform.localPosition += makeVector * makeRange;
            crystal.transform.LookAt(this.transform);
        }
    }
}
