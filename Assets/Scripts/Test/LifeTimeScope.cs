using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ShotLifeTimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        // View / Infra をインターフェイスとして登録
        builder.RegisterComponentInHierarchy<ShotPresentation>().As<IShotPresentation>();
        builder.RegisterComponentInHierarchy<ShotInfrastructure>().As<IShotInfrastructure>();

        // Domain
        builder.Register<ShotDomain>(Lifetime.Scoped).As<IShotDomain>();

        // Application
        builder.Register<ShotApplication>(Lifetime.Scoped)
            .As<IShotApplication>()
            .As<IStartable>();
    }
}
