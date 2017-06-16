using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;

namespace EZRAC.Core.Caching
{
    /// <summary>
    /// A façade that handles caching by delegating the actions to the configured 
    /// <see cref="ICacheProvider"/> instance. The <see cref="ICacheProvider"/> instance 
    /// to be used, needs to be defined using a dependency injection provider. The ServiceLocator is used 
    /// to ask the dependency container for the current instance.
    /// </summary>
    public static class Cache
    {
        #region Field(s)

        private static readonly ICacheProvider CacheProvider;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Static constructor that is used to initialize the static CacheProvider (ICacheProvider) 
        /// object. This static constructor is only performed once. It is called 
        /// automatically before the first instance is created or any static members 
        /// are referenced.
        /// </summary>
        static Cache()
        {
			CacheProvider = ServiceLocator.Current.GetInstance<ICacheProvider>();
        }

        #endregion

        #region Method(s)

	    /// <summary>
	    /// Stores an object in the cache. Throws an exception if an object with the same key is already in the cache.
	    /// </summary>
	    /// <param name="key">The unique value that is used to identify the object in the cache.</param>
	    /// <param name="value">The object to add.</param>
	    /// <param name="timeout">The amount of time the object should reside in the cache before it expires.</param>
	    /// <param name="region">The region in which the object should be stored.</param>
	    /// <exception cref="ArgumentNullException">key or value are null</exception>
	    /// <exception cref="ArgumentException">An item with the same key is already present in the cache -or- the 
	    /// specified region does not exist</exception>
        public static void Add<T>(string key, T item, DateTime absoluteExpiration, TimeSpan slidingExpiration)
		{
            CacheProvider.Add(key, item, absoluteExpiration, slidingExpiration);
		}	


        /// <summary>
        /// Adds an item to the cache.
        /// </summary>
        /// <param name="key">The key used to identify the item in the cache.</param>
        /// <param name="item">The item to be stored in the cache.</param>
        public static void Add<T>(string key, T item)
        {
            CacheProvider.Add(key, item);
        }

		/// <summary>
		/// Retrieves the object with the given key from the cache.
		/// </summary>
		/// <param name="key">The unique value that is used to identify the object in the cache.</param>
		/// <param name="region">The region in which the object is stored.</param>
		/// <returns>The object that was cached with the specified key -or- null if the key does not exist.</returns>
		/// <exception cref="ArgumentNullException">key is null</exception>
		/// <exception cref="ArgumentException">the specified region does not exist</exception>
        public static T Get<T>(string key)
        {
            return CacheProvider.Get<T>(key);
        }

        public static void Remove(string key)
        {
             CacheProvider.Remove(key);
        }

        public static void Clear()
        {
            CacheProvider.Clear();
        }
        #endregion
    }
}