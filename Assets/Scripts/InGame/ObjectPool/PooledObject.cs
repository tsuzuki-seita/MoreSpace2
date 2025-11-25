using System.Threading; // CancellationToken用
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ObjectPool
{
    public class PooledObject : MonoBehaviour
    {
        private IPool _pool;
        public IPool Pool { get => _pool; set => _pool = value; }
        private CancellationTokenSource _cts;

        protected void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }

        protected void Release()
        {
            CancelDelayedRelease();
            _pool.Return(this);
        }

        protected void Release(float time)
        {
            CancelDelayedRelease();
            _cts = new CancellationTokenSource();
            ReleaseAsync(time, _cts.Token).Forget();
        }

        private void CancelDelayedRelease()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }
        
        private async UniTaskVoid ReleaseAsync(float time, CancellationToken token)
        {
            var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(
                token, 
                this.GetCancellationTokenOnDestroy()
            ).Token;

            bool canceled = await UniTask.WaitForSeconds(time, cancellationToken: linkedToken).SuppressCancellationThrow();

            if (canceled) return;

            _pool?.Return(this);
            if (_cts != null && _cts.Token == token)
            {
                _cts.Dispose();
                _cts = null;
            }
        }
    }
}