namespace MoreSpace.InGame
{
    public interface IDamageable
    {
        public abstract void Damage(int damage);
        public abstract void Die(Photon.Realtime.Player doPlayer);
    }
}
