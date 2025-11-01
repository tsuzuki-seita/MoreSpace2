using System;
using UnityEngine;

namespace ObjectPool
{
    public class PooledObject : MonoBehaviour
    {
        private IPool _pool;
        public IPool Pool { get => _pool; set => _pool = value; }

        protected void Release()
        {
            _pool.Return(this);
        }
    }
}