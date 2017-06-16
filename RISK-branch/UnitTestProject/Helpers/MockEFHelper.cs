using System;
using System.Collections.Generic;
using Moq;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EZRAC.Core.Data.EntityFramework;
using EZRAC.Risk.Services.Test.Helpers;
using System.Threading;
using FizzWare.NBuilder;

namespace UnitTestProject.Helpers
{
    public class MockEFHelper: IEFHelper
    {
        public IEnumerable<T> ExecuteSQLQuery<T>(string query)
        {
            return Builder<T>.CreateListOfSize(5).All().Build().ToList();
        }

        public IEnumerable<T> ExecuteSQLQuery<T>(string query, params object[] parameters) 
        {
            return Builder<T>.CreateListOfSize(5).All().Build().ToList();
        }

        public async Task<IEnumerable<T>> ExecuteSQLQueryAsync<T>(string query, params object[] parameters) 
        {
            return Builder<T>.CreateListOfSize(5).All().Build().ToList();
        }

        public async Task<IEnumerable<T>> ExecuteSQLQueryAsync<T>(string query)
        {
            return Builder<T>.CreateListOfSize(5).All().Build().ToList();
        }

       ///

        //public IEFHelper SetUpRepository<TEntity>()
        //{
        //    Mock<IEFHelper> mockEFHelper = new Mock<IEFHelper>();

        //    mockEFHelper.Setup(x => x.ExecuteSQLQuery<TEntity>(It.IsAny<string>())).Returns(DtoBuilder.GetList<TEntity>());

        //    mockEFHelper.Setup(x => x.ExecuteSQLQuery<TEntity>(It.IsAny<string>(), It.IsAny<ParamArrayAttribute>())).Returns(DtoBuilder.GetList<TEntity>());

        //    mockEFHelper.Setup(x => x.ExecuteSQLQueryAsync<TEntity>(It.IsAny<string>(), It.IsAny<ParamArrayAttribute>())).ReturnsAsync(DtoBuilder.GetList<TEntity>());

        //    mockEFHelper.Setup(x => x.ExecuteSQLQueryAsync<TEntity>(It.IsAny<string>())).ReturnsAsync(DtoBuilder.GetList<TEntity>());

        //    mockEFHelper.Setup(x => x.ExecuteSQLQueryAsync<TEntity>(It.IsAny<string>(), It.IsAny<ParamArrayAttribute>()));

        //    mockEFHelper.Setup(x => x.ExecuteSQLQuery<TEntity>(It.IsAny<string>(), It.IsAny<ParamArrayAttribute>()));

        //    return mockEFHelper.Object;

        //}
    }
}
