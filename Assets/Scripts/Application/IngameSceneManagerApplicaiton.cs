// Application/Interfaces.cs
using MoreSpace.Domain;
using UnityEngine.SceneManagement;

namespace MoreSpace.Application
{
    public interface IUserProfileRepository
    {
        UserProfile Load();
        void Save(UserProfile profile);
    }

    // “読んだら消える”一時引数バス
    public interface ISceneArgsBus
    {
        void Publish<T>(T value);
        bool TryConsume<T>(out T value);
    }

    // ナビゲーション：永続更新 + 一時引数 + シーンロード
    public interface INavigationService
    {
        void Navigate(InGameState sceneName, object transientArgs = null, UserProfile persistentUpdate = null);
    }

    public sealed class NavigationService : INavigationService
    {
        private readonly ISceneArgsBus _bus;
        private readonly IUserProfileRepository _repo;

        public NavigationService(ISceneArgsBus bus, IUserProfileRepository repo)
        {
            _bus = bus;
            _repo = repo;
        }

        public void Navigate(InGameState sceneName, object transientArgs = null, UserProfile persistentUpdate = null)
        {
            // 永続更新（必要なときだけ）
            if (persistentUpdate != null)
            {
                _repo.Save(persistentUpdate);
            }

            // 一時引数（型そのまま Publish<T>）
            if (transientArgs != null)
            {
                var m = typeof(ISceneArgsBus).GetMethod(nameof(ISceneArgsBus.Publish))!;
                var gm = m.MakeGenericMethod(transientArgs.GetType());
                gm.Invoke(_bus, new[] { transientArgs });
            }

            SceneManager.LoadScene(sceneName.ToString());
        }
    }
}
