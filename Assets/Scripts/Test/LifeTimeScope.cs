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
        // Core services
        builder.Register<IUserProfileRepository, PlayerPrefsUserProfileRepository>(Lifetime.Singleton);
        builder.Register<ISceneArgsBus, SceneArgsBus>(Lifetime.Singleton);
        builder.Register<INavigationService, NavigationService>(Lifetime.Singleton);

        // Scene singleton (MonoBehaviour) をコンテナに登録
        if (ingameSceneManagerInHierarchy == null)
            ingameSceneManagerInHierarchy = FindObjectOfType<IngameSceneManager>(true);

        builder.RegisterComponent(ingameSceneManagerInHierarchy);
    }
}