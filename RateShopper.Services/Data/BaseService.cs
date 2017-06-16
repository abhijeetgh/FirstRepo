using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateShopper.Data;
using System.Data.Entity;
using RateShopper.Core.Cache;
using System.Data.Entity.Validation;

namespace RateShopper.Services.Data
{
    public class BaseService<T> : IDisposable, IBaseService<T> where T : BaseEntity
    {
        protected IEZRACRateShopperContext _context;
        protected ICacheManager _cacheManager;
        protected IDbSet<T> _dbset;

        public BaseService(IEZRACRateShopperContext context, ICacheManager cacheManager)
        {
            _context = context;
            _dbset = _context.Set<T>();
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// Add Entity
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Add(T entity)
        {
            if (entity == null)
            {
                return;

            }
            else
            {
                _dbset.Add(entity);
                _context.SaveChanges();
                _cacheManager.Remove(typeof(T).ToString());

            }

        }

        /// <summary>
        /// Delete Entity
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Delete(T entity)
        {
            if (entity == null)
            {
                return;

            }
            else
            {
                _dbset.Remove(entity);
                _context.SaveChanges();
                _cacheManager.Remove(typeof(T).ToString());
            }

        }

        /// <summary>
        /// Delete entity by ID
        /// </summary>
        /// <param name="id"></param>
        public virtual void Delete(long id)
        {
            if (id < 0)
            {
                return;

            }
            else
            {
                T entity = _dbset.Find(id);
                Delete(entity);
            }

        }

        /// <summary>
        /// Get entity by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetById(long id, bool isCacheble = true)
        {
            if (id < 0)
            {
                return null;

            }
            else
            {
                if (isCacheble)
                {
                    string cacheKey = typeof(T).ToString() + "_" + id;
                    T entity = _cacheManager.Get(cacheKey) as T;
                    if (entity == null)
                    {
                        entity = _dbset.Find(id);
                        _cacheManager.Set(cacheKey, entity);
                    }
                    return entity;
                }

                return _dbset.Find(id);
            }

        }

        /// <summary>
        /// Get all entities
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<T> GetAll(bool isCacheble = true)
        {
            if (isCacheble)
            {
                Object thisLock = new Object();

                lock (thisLock)
                {
                    IEnumerable<T> entities = _cacheManager.Get(typeof(T).ToString()) as IEnumerable<T>;
                    if (entities == null)
                    {
                        entities = _dbset;
                        _cacheManager.Set(typeof(T).ToString(), entities);
                    }

                    return entities.ToList();
                }

            }

            return _dbset.ToList();
        }

        /// <summary>
        /// Update Entity
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Update(T entity)
        {
            if (entity == null)
            {
                return;

            }
            else
            {
                _context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                _cacheManager.Remove(typeof(T).ToString());
            }

        }

        /// <summary>
        /// Return Count
        /// </summary>
        /// <returns></returns>
        public virtual int Count()
        {
            return _dbset.ToList().Count();
        }

        //public void Dispose()
        //{
        //    if (null != _context)
        //        _context = null;
        //}
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (null != _context)
                    _context.Dispose();
                    _context = null;
            }
        }
    }
}
