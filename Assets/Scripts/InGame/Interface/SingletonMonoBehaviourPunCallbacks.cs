using Photon.Pun;
using UnityEngine;

namespace MoreSpace.InGame
{
    public class SingletonMonoBehaviourPunCallbacks<T> : MonoBehaviourPunCallbacks where T : MonoBehaviourPunCallbacks
    {
        private static T _instance;

        public static T Instance
        {
            get 
            {
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = FindFirstObjectByType<T>();

                if (_instance == null)
                {
                    var go = new GameObject(typeof(T).Name);

                    _instance = go.AddComponent<T>();
                }

                return _instance;
            }
        }
    }

}