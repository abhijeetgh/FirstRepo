using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using EZRAC.Core.Caching;

namespace TestClient
{
    public class TestCaching
    {
        ICacheProvider cacheProvider = Program.container.Resolve<ICacheProvider>();

        public void AddCache()
        {
            
            cacheProvider.Add("sri","test");
        }

        public void GetCache()
        {
            var test= cacheProvider.Get<string>("sri");
        }

    }
}
