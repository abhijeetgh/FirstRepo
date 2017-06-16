using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.Core.Data.EntityFramework
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {

        Task<TEntity> GetByIdAsync(Int64 id);

        Task<TEntity> GetByIdAsync(int id);        

        Task<IEnumerable<TEntity>> GetAllAsync();

        Task UpdateAsync(TEntity entity);

        Task BulkUpdateAsync(IEnumerable<TEntity> entities);

        Task<TEntity> InsertAsync(TEntity entity);

        Task DeleteAsync(TEntity entity);

        IEnumerable<TEntity> ExecuteSQLQuery(string  query);

        IQueryable<TEntity> AsQueryable { get; }
    } 
}
