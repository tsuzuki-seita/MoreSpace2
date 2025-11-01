using System.Collections.Generic;
using UnityEngine;

namespace ObjectPool
{
    public class Pool<T> : IPool where T : PooledObject
    {
        private T objectToPool;
        private Stack<T> stack;

        public Pool(uint initSize, T target)
        {
            objectToPool = target;
            SetupPool(initSize);
        }

        private void SetupPool(uint initSize)
        {
            // missing objectToPool Prefab field
            if (objectToPool == null)
            {
                return;
            }

            stack = new Stack<T>();

            T instance;
            for (int i = 0; i < initSize; i++)
            {
                instance = MakeObject();
                instance.gameObject.SetActive(false);
                stack.Push(instance);
            }
        }

        T MakeObject()
        {
            var instance = Object.Instantiate(objectToPool);
            instance.Pool = this;
            return instance;
        }

        public T GetPooledObject()
        {
            if (objectToPool == null)
            {
                return null;
            }

            if (!stack.TryPop(out var nextInstance))
            {
                nextInstance = MakeObject();
                return nextInstance;
            }

            nextInstance.gameObject.SetActive(true);
            return nextInstance;
        }

        // returns the GameObject to the pool
        private void ReturnToPool(T pooledObject)
        {
            stack.Push(pooledObject);
            pooledObject.gameObject.SetActive(false);
        }

        public void Return(PooledObject instance)
        {
            ReturnToPool(instance as T);
        }
    }

    public interface IPool
    {
        void Return(PooledObject instance);
    } 
}