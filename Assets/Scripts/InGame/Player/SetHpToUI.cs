using MoreSpace.InGame;
using UnityEngine;
using UnityEngine.UI;

namespace MoreSpace.InGame.Player
{
    public class SetHpToUI : MonoBehaviour
    {
        [SerializeField] private Scrollbar hpBar;
        [SerializeField] private HealthBase target;

        void Start()
        {
            target.OnDamage += ChangeValue;
        }

        private void OnDestroy()
        {
            target.OnDamage -= ChangeValue;
        }

        void ChangeValue(int hp, int maxHp)
        {
            hpBar.size = (float)hp / maxHp;
        }
    }
}