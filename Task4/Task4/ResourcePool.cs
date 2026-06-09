using System.Collections.Concurrent;

namespace Task4
{
    internal class ResourcePool
    {
        private readonly ConcurrentStack<object> _pool = new();
        private SemaphoreSlim _sync;
        private int _maxResources;

        public ResourcePool(int maxResources)
        {
            _maxResources = maxResources;
            _sync = new SemaphoreSlim(maxResources, maxResources);
            for (int i = 0; i < maxResources; i++) _pool.Push(new object());
        }

        public int AvailableCount
        {
            get { return _pool.Count; }
        }

        public void AcquireResource()
        {
            _sync.Wait();

            _pool.TryPop(out var obj);

            _sync.Release();
        }

        public void ReleaseResource()
        {
            _sync.Wait();

            _pool.Push(new object());

            _sync.Release();
        }

        public bool TryAcquireResource(int timeoutMs)
        {
            bool lockTaken = _sync.Wait(timeoutMs);
            if (lockTaken)
            {
                _sync.Release();
            }
            return lockTaken;
        }
    }
}
