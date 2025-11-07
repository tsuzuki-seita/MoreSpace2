using UnityEngine;
using VContainer;
using VContainer.Unity;
using MoreSpace.Application;     // Service, Interface
using MoreSpace.Infrastructure;   // Repository, Bus
using MoreSpace.Presentation;   // Presenter, SceneManager

public class SelectSkillLifeTimeScope : LifetimeScope
{
    [SerializeField] private IngameSceneManager ingameSceneManagerInHierarchy;


    protected override void Configure(IContainerBuilder builder)
    {
        // -------------------------------------------------
        // 1. Core Services (Application/Infrastructure)
        // -------------------------------------------------
        // これらはシーンをまたいでデータを保持・管理する必要があるため、
        // Singleton (実質DontDestroyOnLoad) として登録します。
        // もし親のScope (ProjectContextなど) で既に登録済みなら、
        // VContainerが自動でそちらを使用するため、二重登録にはなりません。

        // 永続データ(PlayerPrefs)のI/Fと実装
        builder.Register<IUserProfileRepository, PlayerPrefsUserProfileRepository>(Lifetime.Singleton);

        // シーン間引数バスのI/Fと実装
        builder.Register<ISceneArgsBus, SceneArgsBus>(Lifetime.Singleton);

        // シーン遷移サービス本体 (上記2つに依存)
        builder.Register<NavigationService>(Lifetime.Singleton);


        // -------------------------------------------------
        // 2. Scene-Specific Services (Application/Infrastructure)
        // -------------------------------------------------
        // これらは「スキル選択シーン」でのみ必要なサービスです。
        // Scoped にすることで、このシーンがロードされた時に生成され、
        // シーンがアンロードされる時に破棄されます。

        // スキルデータ(Resources)のI/Fと実装
        builder.Register<ISkillRepository, ResourceSkillRepository>(Lifetime.Scoped);

        // スキル選択シーンのユースケース (NavigationService と ISkillRepository に依存)
        builder.Register<SkillSelectionService>(Lifetime.Scoped);


        // -------------------------------------------------
        // 3. Presentation (MonoBehaviour)
        // -------------------------------------------------
        // シーンに配置されているMonoBehaviourを登録し、
        // 依存関係 (Serviceなど) を自動で注入させます。

        // スキル選択UIのPresenter
        // (SkillSelectionService が [Inject] されます)
        builder.RegisterComponentInHierarchy<SkillSelectionPresenter>();

        // シーンマネージャー (シングルトンインスタンス)
        // (NavigationService, ISceneArgsBus, IUserProfileRepository が [Inject] されます)
        // ※IngameSceneManagerがこのシーンに配置されている場合
        if (ingameSceneManagerInHierarchy == null)
            ingameSceneManagerInHierarchy = FindObjectOfType<IngameSceneManager>(true);
        builder.RegisterComponent(ingameSceneManagerInHierarchy);
    }
}
