using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EZRAC.Core.Caching.ContextCache
{
    public class ContextCache : ICacheProvider
    { 
        /// <summary>
        /// Adds an item to the cache.
        /// </summary>
        /// <param name="key">The key used to identify the item in the cache.</param>
        /// <param name="item">The item to be stored in the cache.</param>
        /// <param name="absoluteExpiration">The time at which the item expires and can be removed from the cache.</param>
        /// <param name="slidingExpiration">The interval between the time the item is last accessed and when that item expires and can be removed from the cache.</param>
        public void Add<T>(string key, T item, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        { 
           if(!Exists(key))
           {
               if (HttpContext.Current != null && HttpContext.Current.Cache != null)
               {
                   HttpContext.Current.Cache.Insert(key, item, null, absoluteExpiration, slidingExpiration);                   
               }
           }
        }

        /// <summary>
        /// Adds an item to the cache.
        /// </summary>
        /// <param name="key">The key used to identify the item in the cache.</param>
        /// <param name="item">The item to be stored in the cache.</param>
        public void Add<T>(string key, T item)
        {
            if (!Exists(key))
            {
                if (HttpContext.Current != null && HttpContext.Current.Cache != null)
                {
                    HttpContext.Current.Cache.Insert(key, item, null, DateTime.MaxValue, TimeSpan.Zero);                    
                }
            }
        }

        /// <summary>
        /// Determines whether the specified key refers to an existing item in the cache.
        /// </summary>
        /// <param name="key">The identifier for the cache item to check for.</param>
        /// <returns><b>true</b> if path refers to an existing cached item; otherwise, <b>false</b>.</returns>
        public bool Exists(string key)
        {
            if (HttpContext.Current != null)
                return HttpContext.Current.Cache[key] != null ? true : false;
            else
                return false;
        }

        /// <summary>
        /// Gets the specified item from the cache.
        /// </summary>
        /// <param name="key">The identifier for the cache item to retrieve.</param>
        /// <returns>The retrieved cache item, or a null reference if the key is not found.</returns>
        public  T Get<T>(string key)
        {
            T item = default(T);

            if (HttpContext.Current == null || HttpContext.Current.Cache == null)
                return item;

            if (string.IsNullOrEmpty(key))
                return item;


            if (HttpContext.Current.Cache[key] != null && HttpContext.Current.Cache[key].GetType() == typeof(T))
            {
                item = (T)HttpContext.Current.Cache[key];
            }

            return item;
        }

        /// <summary>
        /// Deletes all items contained in the cache.
        /// </summary>
        public void Clear()
        {
            IDictionaryEnumerator enumerator = HttpContext.Current.Cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                HttpContext.Current.Cache.Remove(enumerator.Key.ToString());                
            }
        }

        /// <summary>
        /// Removes the specified item from the cache.
        /// </summary>
        /// <param name="key">The identifier for the cache item to remove.</param>
        public void Remove(string key)
        {
            HttpContext.Current.Cache.Remove(key);            
        }
        
    }
}
