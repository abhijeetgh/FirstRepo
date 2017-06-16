using System;
using System.Collections.Generic;

namespace RateShopper.Core.Cache
{
    public interface ICacheManager
    {
        void Set(string key, object value);
        void Set(string key, object value, DateTimeOffset absoluteExpiration);

        object Get(string key);

        void Remove(string key);

        void Remove(List<string> keys);

        void RemoveAllCacheObjects();
    }
}