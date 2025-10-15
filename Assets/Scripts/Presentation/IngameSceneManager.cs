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
    Pause
}

namespace MoreSpace.Presentation
{
    public sealed class IngameSceneManager : MonoBehaviour
    {
        public static IngameSceneManager Instance { get; private set; }

        [SerializeField] private bool dontDestroyOnLoad = true;

        private INavigationService _nav;
        private IUserProfileRepository _repo;
        private ISceneArgsBus _bus;

        // VContainer から注入
        [Inject]
        public void Construct(INavigationService nav, IUserProfileRepository repo, ISceneArgsBus bus)
        {
            _nav = nav;
            _repo = repo;
            _bus = bus;
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
        }

        // ========== 公開API（ユーティリティ） ==========

        // ユーザーネームの取得（永続）
        public string LoadUserName() => _repo.Load().UserName;

        // ユーザーネームを保存（永続）
        public void SaveUserName(string name) => _repo.Save(new UserProfile(name));

        // 任意シーンへ：永続を更新しつつ一時引数も渡す
        public void NavigateWith(InGameState sceneName, object transientArgs = null, string newUserName = null)
        {
            UserProfile persist = null;
            if (newUserName != null) persist = new UserProfile(newUserName);
            _nav.Navigate(sceneName, transientArgs, persist);
        }

        // 便利：インゲームへ（スキル3つ + 必要なら名前も更新）
        public void GoToIngame(SkillLoadout loadout, string newUserName = null)
        {
            var args = new IngameArgs(loadout);
            //NavigateWith("Ingame", args, newUserName);
        }

        // 受け取り側用：IngameArgsを“読んだら消える”で取得
        public bool TryConsumeIngameArgs(out IngameArgs args)
            => _bus.TryConsume(out args);
    }
}
