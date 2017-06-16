using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.Core.Data.EntityFramework
{
    public interface IBaseRepository<T> where T  : class
    {
        IQueryable<T> AsQueryable { get; }
        IEnumerable<T> GetAll();                
        void Save(T entity);
        void Delete(T entity);
        void Delete(Expression<Func<T, bool>> predicate);        
        void Update(T entity);
        T GetById(long id);        
    }
}
