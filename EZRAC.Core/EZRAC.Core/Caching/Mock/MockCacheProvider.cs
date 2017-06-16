using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace EZRAC.Core.Caching.Mock
{
    /// <summary>
    /// A cache provider that uses an in-memory collection to store objects. Expiration timeout is not implemented, 
    /// suitable for testing.
    /// </summary>
    public class MockCacheProvider : ICacheProvider
    {
        private static ObjectCache _cache = MemoryCache.Default;

        public void Add<T>(string key, T item, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            if (!_cache.Contains(key))
            {
                _cache.Set(key, item,absoluteExpiration);
            }
        }

        public void Add<T>(string key, T item)
        {
            if (!_cache.Contains(key))
            {
                _cache.Set(key, item, new CacheItemPolicy());
            }
        }

        public bool Exists(string key)
        {
            if (_cache != null)
                return _cache[key] != null ? true : false;
            else
                return false;
        }

        public T Get<T>(string key)
        {
            return (T)_cache.Get(key);
        }

        public void Clear()
        {
             if (_cache != null)
             {
                 foreach (var element in MemoryCache.Default)
                 {
                     _cache.Remove(element.Key);
                 }   
             }
        }

        public void Remove(string key)
        {
            if (_cache.Contains(key))
            {
                _cache.Remove(key);
            }
        }
    }
}