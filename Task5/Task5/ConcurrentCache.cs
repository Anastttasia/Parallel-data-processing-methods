using System.Collections.Concurrent;

namespace Task5
{

    class CacheItem
    {
        public object Value;
        private TimeSpan ExpirationTime;
        private DateTime Timestamp;

        public CacheItem(object Value)
        {
            Timestamp = DateTime.Now;

            this.Value = Value;
            this.ExpirationTime = TimeSpan.MaxValue;
        }
    }

    internal class ConcurrentCache
    {

        private ConcurrentDictionary<string, ConcurrentBag<CacheItem>> Cache = new ConcurrentDictionary<string, ConcurrentBag<CacheItem>>();

        public bool AddToCache(string key, object value)
        {
            if (Cache.ContainsKey(key)) return false;

            CacheItem item = new CacheItem(value);

            ConcurrentBag<CacheItem> bag = new ConcurrentBag<CacheItem>();

            bag.Add(item);

            return Cache.TryAdd(key, bag);
        }
        public bool TryGetFromCache(string key, out object value)
        {
            if (!Cache.ContainsKey(key))
            {
                value = new object();
                return false;
            }

            ConcurrentBag<CacheItem> bag = Cache[key];
            CacheItem? cacheItem;

            if (!bag.TryTake(out cacheItem))
            {
                value = new object();
                return false;
            }

            value = cacheItem.Value;

            return true;
        }

        public bool RemoveFromCache(string key)
        {
            if (Cache.ContainsKey(key))
            {
                ConcurrentBag<CacheItem> bag;

                return Cache.TryRemove(key, out bag);
            }

            return false;
        }

        public int GetCacheSize()
        {

            return Cache.Count;
        }

        public void ClearCache()
        {

            Cache.Clear();
        }


    }

}
