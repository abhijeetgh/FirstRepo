using System;
using System.Collections.Generic;

namespace EZRAC.Core.Caching
{
    /// <summary>
    /// The <see cref="ICacheProvider"/> interface defines the contract which cache providers must implement.
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
        /// Adds an item to the cache.
        /// </summary>
        /// <param name="key">The key used to identify the item in the cache.</param>
        /// <param name="item">The item to be stored in the cache.</param>
        /// <param name="absoluteExpiration">The time at which the item expires and can be removed from the cache.</param>
        /// <param name="slidingExpiration">The interval between the time the item is last accessed and when that item expires and can be removed from the cache.</param>
        void Add<T>(string key, T item, DateTime absoluteExpiration, TimeSpan slidingExpiration);

        /// <summary>
        /// Adds an item to the cache.
        /// </summary>
        /// <param name="key">The key used to identify the item in the cache.</param>
        /// <param name="item">The item to be stored in the cache.</param>
        void Add<T>(string key, T item);

        /// <summary>
        /// Determines whether the specified key refers to an existing item in the cache.
        /// </summary>
        /// <param name="key">The identifier for the cache item to check for.</param>
        /// <returns><b>true</b> if path refers to an existing cached item; otherwise, <b>false</b>.</returns>
        bool Exists(string key);

        /// <summary>
        /// Gets the specified item from the cache.
        /// </summary>
        /// <param name="key">The identifier for the cache item to retrieve.</param>
        /// <returns>The retrieved cache item, or a null reference if the key is not found.</returns>
        T Get<T>(string key);

        /// <summary>
        /// Deletes all items contained in the cache.
        /// </summary>
        void Clear();

        /// <summary>
        /// Removes the specified item from the cache.
        /// </summary>
        /// <param name="key">The identifier for the cache item to remove.</param>
        void Remove(string key);

        
    }
}
