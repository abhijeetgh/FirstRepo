using EZRAC.Core.Data.EntityFramework;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using EZRAC.Risk.Services.Test.Helpers;

namespace EZRAC.Risk.Services.Test.Helpers
{
    public class MockGenericRepository<TEntity> where TEntity : class
    {
        private List<TEntity> _data = null;

        public MockGenericRepository(List<TEntity> mockData)
        {

            _data = mockData;
        }

        public IGenericRepository<TEntity> SetUpRepository()
        {
           

            var dbSet = GenericSetupAsyncQueryableMockSet<TEntity>(_data.AsQueryable());

            Mock<IGenericRepository<TEntity>> mockRepository = new Mock<IGenericRepository<TEntity>>();


            mockRepository.Setup(x => x.AsQueryable).Returns(dbSet.Object);

            mockRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(dbSet.Object.FirstOrDefault());

            mockRepository.Setup(x => x.GetByIdAsync(It.IsAny<long>())).ReturnsAsync(dbSet.Object.FirstOrDefault());

            mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(dbSet.Object.AsEnumerable());


            mockRepository.Setup(x => x.UpdateAsync(It.IsAny<TEntity>())).Returns(Task.FromResult(true));

            

            mockRepository.Setup(x => x.BulkUpdateAsync(It.IsAny<IEnumerable<TEntity>>())).Returns(Task.FromResult(true));

            mockRepository.Setup(x => x.InsertAsync(It.IsAny<TEntity>())).ReturnsAsync(dbSet.Object.FirstOrDefault());
          

            
            mockRepository.Setup(x => x.DeleteAsync(It.IsAny<TEntity>())).Returns(Task.FromResult(true));

            return mockRepository.Object;
        }

      
        private Mock<IDbSet<T>> GenericSetupAsyncQueryableMockSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<IDbSet<T>>();

            mockSet.As<IDbAsyncEnumerable<T>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbSet.AsyncEnumeratorWrapper<T>(data.GetEnumerator()));
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new MockDbSet.AsyncQueryProviderWrapper<T>(data.Provider));
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            return mockSet;
        }

       
    }
}
