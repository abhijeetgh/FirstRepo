using System;
using System.Runtime.Caching;

namespace RateShopper.Core.Cache
{
    public class InMemoryCacheManager : ICacheManager
    {
        private static ObjectCache _cache = MemoryCache.Default;

        public void Set(string key, object value)
        {
            if (!_cache.Contains(key))
            {
                _cache.Set(key, value, new CacheItemPolicy());
            }
        }

        public void Set(string key, object value, DateTimeOffset absoluteExpiration)
        {
            if (!_cache.Contains(key))
            {
                _cache.Set(key, value, absoluteExpiration);
            }
        }

        public object Get(string key)
        {
            return _cache.Get(key);
        }

        public void Remove(string key)
        {
            if (_cache.Contains(key))
            {
                _cache.Remove(key);
            }
        }

        public void Remove(System.Collections.Generic.List<string> keys)
        {
            foreach (var key in keys)
            {
                Remove(key);
            }
        }

        public void RemoveAllCacheObjects()
        {
            foreach (var element in MemoryCache.Default)
            {
                Remove(element.Key);
            }            
        }

    }
}