using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Data
{
    public interface IBaseService<T> where T : BaseEntity
    {
        void Add(T entity);

        void Delete(T entity);

        void Delete(long id);

        T GetById(long id, bool isCacheble = true);

        IEnumerable<T> GetAll(bool isCacheble = true);

        void Update(T entity);

        int Count();

    }
}
