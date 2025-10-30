using MoreSpace.InGame;
using Unity.VisualScripting;
using UnityEngine;
using MoreSpace.InGame;

namespace MoreSpace.InGame.Player
{
    public class PlayerHp : MonoBehaviour, IDamageable
    {
        [SerializeField] int playerHp = 100;
        public void Damage(int damage)
        {
            playerHp -= damage;
            Debug.Log($"{damage}受けています, 残りHP: {playerHp}");
            if (playerHp <= 0)
            {
                Debug.Log("Player is dead.");
            }
        }
        private void Die()
        {
            Debug.Log("Player is dead. プレイヤーを破壊するなどの処理をここに追加。");
        }
    }
}

