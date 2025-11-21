using MoreSpace.InGame;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MoreSpace.InGame.Player
{
    public class SetHpToUI : MonoBehaviour
    {
        [SerializeField] private Scrollbar hpBar;
        [SerializeField] private Scrollbar redDamageBar;
        [SerializeField] private HealthBase target;
        [SerializeField] private bool isLookPlayer;
        [SerializeField] private float damageDrainSpeed = 0.5f;
        private int _currentHp;
        private Coroutine _drainCoroutine;

        void Start()
        {
            if (isLookPlayer)
                FindAnyObjectByType<LookUiToCamera>().AssertUI(hpBar.transform);
            target.OnDamage += ChangeValue;
            _currentHp = target.hp;
        }

        private void OnDestroy()
        {
            target.OnDamage -= ChangeValue;
        }

        void ChangeValue(int newHp, int maxHp)
        {
            redDamageBar.size = (float)_currentHp / maxHp;
            _currentHp = newHp;
            hpBar.size = (float)newHp / maxHp;
            if (_drainCoroutine != null)
            {
                StopCoroutine(_drainCoroutine);
            }
            _drainCoroutine = StartCoroutine(DrainRedDamage(maxHp));
        }
        IEnumerator DrainRedDamage(int maxHp)
        {
            float targetSize = (float)_currentHp / maxHp;
            float currentRedSize = redDamageBar.size;
            float timer = 0f;

            yield return new WaitForSeconds(1.0f);

            while (timer < 1f)
            {
                timer += Time.deltaTime * damageDrainSpeed;

                redDamageBar.size = Mathf.Lerp(currentRedSize, targetSize, timer);

                hpBar.size = targetSize;

                yield return null;
            }

            redDamageBar.size = targetSize;
            hpBar.size = targetSize;
            _drainCoroutine = null;
        }
    }
}