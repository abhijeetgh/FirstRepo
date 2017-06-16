using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.Core.Data.EntityFramework
{
    public class EFHelper : IEFHelper
    {
        private readonly DbContext _dbContext;

        public EFHelper(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Synchronosuly execute query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public IEnumerable<T> ExecuteSQLQuery<T>(string query)
        {
            return _dbContext.Database.SqlQuery<T>(query);
        }

        /// <summary>
        /// Synchronosuly execute query with Paratmers
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IEnumerable<T> ExecuteSQLQuery<T>(string query, params object[] parameters)
        {
            return _dbContext.Database.SqlQuery<T>(query, parameters);
        }

        /// <summary>
        /// Asynchronosly execute the SQL query, Stored Procedure with Parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> ExecuteSQLQueryAsync<T>(string query, params object[] parameters)
        {
            return await _dbContext.Database.SqlQuery<T>(query, parameters).ToListAsync();
        }

        /// <summary>
        /// Asynchronosly execute the SQL query, Stored Procedure
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> ExecuteSQLQueryAsync<T>(string query)
        {
            return await _dbContext.Database.SqlQuery<T>(query).ToListAsync();
        }

        /// <summary>
        /// Asynchronosly execute the SQL query (Fire & Forgot)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></para
        public async Task ExecuteSQLQueryAsync(string query, params object[] parameters)
        {
            await _dbContext.Database.ExecuteSqlCommandAsync(query, parameters);
        }

        /// <summary>
        /// synchronosly execute the SQL query (Fire & Forgot)
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        public void ExecuteSQLQuery(string query, params object[] parameters)
        {
            _dbContext.Database.ExecuteSqlCommand(query, parameters);
        }

    }
}
