using UnityEngine;

namespace MoreSpace.Tutorial
{
    [RequireComponent(typeof(Collider))]
    public class TutorialStepTrigger : MonoBehaviour
    {
        [SerializeField] private TutorialStepType stepType;
        [SerializeField] private bool triggerOnce = true;

        private bool _alreadyTriggered = false;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (triggerOnce && _alreadyTriggered) return;

            _alreadyTriggered = true;

            var manager = FindFirstObjectByType<TutorialManager>();
            if (manager != null)
            {
                manager.ChangeStep(stepType);
            }
        }
    }
}