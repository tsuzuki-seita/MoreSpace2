// Infrastructure/PlayerPrefsUserProfileRepository.cs
using System;
using System.Collections.Generic;
using MoreSpace.Application;
using MoreSpace.Domain;
using UnityEngine;

namespace MoreSpace.Infrastructure
{
    public sealed class PlayerPrefsUserProfileRepository : IUserProfileRepository
    {
        private const string KeyUserName = "user_profile_username";

        public UserProfile Load()
            => new UserProfile(PlayerPrefs.GetString(KeyUserName, string.Empty));


        public void Save(UserProfile profile)
        {
            PlayerPrefs.SetString(KeyUserName, profile?.UserName ?? string.Empty);
            PlayerPrefs.Save();
        }
    }

    public sealed class SceneArgsBus : ISceneArgsBus
    {
        private readonly Dictionary<Type, object> _store = new();

        public void Publish<T>(T value) => _store[typeof(T)] = value!;

        public bool TryConsume<T>(out T value)
        {
            if (_store.TryGetValue(typeof(T), out var obj) && obj is T t)
            {
                _store.Remove(typeof(T));
                value = t;
                return true;
            }
            value = default!;
            return false;
        }

        // ★追加: 読んでも消えない
        public bool TryGet<T>(out T value)
        {
            if (_store.TryGetValue(typeof(T), out var obj) && obj is T t)
            {
                value = t;
                return true;
            }
            value = default!;
            return false;
        }

        // ★追加: 明示削除
        public void Clear<T>()
        {
            _store.Remove(typeof(T));
        }
    }
}
