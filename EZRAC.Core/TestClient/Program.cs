using EZRAC.Core.Caching;
using EZRAC.Core.Caching.ContextCache;
using EZRAC.Core.Caching.Mock;
using EZRAC.Core.Data.EntityFramework;
using EZRAC.Core.Logging;
using EZRAC.Core.Logging.Log4Net;
//using EZRAC.RateShopper.Infrastructure.EntityFramework;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient
{
    class Program
    {
        public static UnityContainer container= new UnityContainer();
        static void Main(string[] args)
        {
            RegisterUnity();

            
            //BaseRepository<UserRole> userRepository = new BaseRepository<UserRole>(new RateShopperContext());
            //var users = userRepository.GetAll();


            //Logger.LogError(new Exception("test"), "test");

            //TestClass.TestLog();
            TestCaching testCaching = new TestCaching();
            testCaching.AddCache();
            testCaching.GetCache();
        }

        public static void RegisterUnity()
        {
            //var container = new UnityContainer();

            //container.RegisterType<ILogger>(new InjectionFactory(factory => LogHelper.GetLogger()));
            container.RegisterType<ILogger, Log4Net>();
            container.RegisterType<ICacheProvider, ContextCache>();    
            UnityServiceLocator locator = new UnityServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => locator);
            
        }
    }
}
