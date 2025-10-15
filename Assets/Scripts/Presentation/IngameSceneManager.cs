// Presentation/IngameSceneManager.cs
using MoreSpace.Application;
using MoreSpace.Domain;
using UnityEngine;
using VContainer;

public enum InGameState
{
    Title,
    Ingame,
    Result,
    GameOver,
    Pause,
    SampleScene
}

namespace MoreSpace.Presentation
{
    public sealed class IngameSceneManager : MonoBehaviour
    {
        public static IngameSceneManager Instance { get; private set; }
        [SerializeField] private bool dontDestroyOnLoad = true;

        private NavigationService _nav;
        private ISceneArgsBus _bus;
        private IUserProfileRepository _repo;

        [Inject]
        public void Construct(NavigationService nav, ISceneArgsBus bus, IUserProfileRepository repo)
        { _nav = nav; _bus = bus; _repo = repo; }

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
        }

        // ★ 追加：遷移のみ
        public void ChangeScene(InGameState sceneName)
            => _nav.ChangeScene(sceneName);

        // 既存：データ受け渡しあり
        public void ChangeScene<T>(InGameState sceneName, T args)
            => _nav.ChangeScene(sceneName, args);

        public bool TryConsume<T>(out T value) => _bus.TryConsume(out value);
        public string LoadUserName() => _repo.Load().UserName;
    }

    // 例1: インゲーム開始用の“一時引数”だけ渡す
    public sealed class IngameArgs : ITransientArgs
    {
        public SkillLoadout Loadout;
        public IngameArgs(SkillLoadout loadout) => Loadout = loadout;
    }

    // 例2: ユーザーネームだけ “永続更新”
    public sealed class UpdateUserName : IPersistentUpdate
    {
        public string NewName;
        public UpdateUserName(string name) => NewName = name;
        public void Apply(IUserProfileRepository repo)
            => repo.Save(new UserProfile(NewName));
    }

    // 例3: “一時 + 永続”の両方を 1つの型で
    public sealed class StartIngameWithProfile : ITransientArgs, IPersistentUpdate
    {
        public SkillLoadout Loadout;
        public string NewUserName;

        public StartIngameWithProfile(SkillLoadout loadout, string newUserName)
        { Loadout = loadout; NewUserName = newUserName; }

        public void Apply(IUserProfileRepository repo)
            => repo.Save(new UserProfile(NewUserName));
    }
}
