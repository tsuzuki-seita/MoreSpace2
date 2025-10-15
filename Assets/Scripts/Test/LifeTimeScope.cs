using MoreSpace.Application;
using MoreSpace.Infrastructure;
using MoreSpace.Presentation;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public sealed class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private IngameSceneManager ingameSceneManagerInHierarchy;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<IUserProfileRepository, PlayerPrefsUserProfileRepository>(Lifetime.Singleton);
            builder.Register<ISceneArgsBus, SceneArgsBus>(Lifetime.Singleton);

            // NavigationService はインターフェースでなく具象登録（メソッドにジェネリックを持つため）
            builder.Register<NavigationService>(Lifetime.Singleton);

            if (ingameSceneManagerInHierarchy == null)
                ingameSceneManagerInHierarchy = FindObjectOfType<IngameSceneManager>(true);
            builder.RegisterComponent(ingameSceneManagerInHierarchy);
    }
}