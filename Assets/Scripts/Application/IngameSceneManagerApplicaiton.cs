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

    // 一時引数であることを示すマーカー
    public interface ITransientArgs { }

    // 永続更新があることを示す契約
    public interface IPersistentUpdate
    {
        void Apply(IUserProfileRepository repo);
    }

    public sealed class NavigationService
    {
        private readonly ISceneArgsBus _bus;
        private readonly IUserProfileRepository _repo;

        public NavigationService(ISceneArgsBus bus, IUserProfileRepository repo)
        { _bus = bus; _repo = repo; }

        public void ChangeScene(InGameState sceneName)
        {
            SceneManager.LoadScene(sceneName.ToString());
        }

        public void ChangeScene<T>(InGameState sceneName, T args)
        {
            // 1) 永続更新があれば先に適用
            if (args is IPersistentUpdate persistent)
            {
                persistent.Apply(_repo);
            }

            // 2) 一時引数であれば、ロード後に消えるデータとして保存
            if (args is ITransientArgs)
            {
                _bus.Publish(args);
            }

            // 3) シーンロード
            SceneManager.LoadScene(sceneName.ToString());
        }
    }
}
