using System;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public interface IShotApplication
{
    public void ShotfromApp(Amo amo);
}

public class ShotApplication : IShotApplication, IStartable
{
    IShotPresentation _shotPresentation;
    IShotInfrastructure _shotInfrastructure;
    IShotDomain _shotDomain;
    IDisposable _subscription;

    [Inject]
    public ShotApplication(IShotPresentation shotPresentation, IShotInfrastructure shotInfrastructure, IShotDomain shotDomain)
    {
        _shotPresentation = shotPresentation;
        _shotInfrastructure = shotInfrastructure;
        _shotDomain = shotDomain;
    }

    // ctor では購読せず、Start で開始
    public void Start()
    {
        _subscription = _shotInfrastructure.Shot
            .Subscribe(_ => {
                var amo = _shotDomain.GetCurrentAmo();  // ← Domain から結果を取得
                _shotPresentation.Shot(amo);            // ← 見た目は Presentation へ
            });
    }

    public void ShotfromApp(Amo amo)
    {
        _shotPresentation.Shot(amo);
    }
}
