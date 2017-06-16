using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.Core.Data.EntityFramework
{
    public interface IEFHelper
    {
        IEnumerable<T> ExecuteSQLQuery<T>(string query);
        IEnumerable<T> ExecuteSQLQuery<T>(string query, params object[] parameters);
        Task<IEnumerable<T>> ExecuteSQLQueryAsync<T>(string query);
        Task<IEnumerable<T>> ExecuteSQLQueryAsync<T>(string query, params object[] parameters);
    }
}
