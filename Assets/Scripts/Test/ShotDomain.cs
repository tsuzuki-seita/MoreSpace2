using UnityEngine;
using VContainer;

public enum Amo
{
    Small,
    Medium,
    Large
}

public interface IShotDomain
{
    void ChangeAmo(Amo amo);
    Amo GetCurrentAmo();
}

public class ShotDomain : IShotDomain
{
    public Amo CurrentAmo { get; private set; } = Amo.Small;

    public ShotDomain()
    {
        
    }

    public void ChangeAmo(Amo amo)
    {
        CurrentAmo = amo;
    }

    public Amo GetCurrentAmo()
    {
        return CurrentAmo;
    }
}
