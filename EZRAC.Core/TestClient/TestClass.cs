using EZRAC.Core.Logging;
using EZRAC.Core.Logging.Log4Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient
{
    public class TestClass
    {
        public static void TestLog()
        {            
            Logger.LogError("MyMessage", "Console", "TestLog()");
        }
    }
}
