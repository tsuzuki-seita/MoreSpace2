using System.Collections.Generic;

namespace MoreSpace.InGame.Master
{
    public static class DamageableHolder
    {
        public static Dictionary<int,IDamageable> Holders = new Dictionary<int, IDamageable>();

        public static IDamageable GetInstance(int viewID)
        {
            return Holders[viewID];
        }
    }
}