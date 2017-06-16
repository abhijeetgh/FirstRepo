using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EZRAC.Core.Data.EntityFramework
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected DbSet<TEntity> DbSet;

        private readonly DbContext _dbContext;

        public GenericRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
            DbSet = _dbContext.Set<TEntity>();
        }

        public GenericRepository()
        {
        }

        public IQueryable<TEntity> AsQueryable
        {
            get
            {
                return DbSet;
            }
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var result = await DbSet.ToListAsync().ConfigureAwait(false);
            return result;
        }

        public async Task<TEntity> GetByIdAsync(Int64 id)
        {
            return await DbSet.FindAsync(id);
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await DbSet.FindAsync(id);
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<TEntity> InsertAsync(TEntity entity)
        {

            entity = DbSet.Add(entity);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            return entity;
        }

        public async Task DeleteAsync(TEntity entity)
        {
            DbSet.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }


        public IEnumerable<TEntity> ExecuteSQLQuery(string query)
        {
            return _dbContext.Database.SqlQuery<TEntity>(query);
        }


        public async Task BulkUpdateAsync(IEnumerable<TEntity> entities)
        {
            foreach (var item in entities)
            {
                _dbContext.Entry(item).State = EntityState.Modified;
            }
            await _dbContext.SaveChangesAsync();
        }
    }

    public static class QueryExtension
    {
        public static IQueryable<T> IncludeMultiple<T>(this IQueryable<T> query, params Expression<Func<T, object>>[] includes)
    where T : class
        {
            if (includes != null)
            {
                query = includes.Aggregate(query,
                          (current, include) => current.Include(include));
            }

            return query;
        }
    }
}
