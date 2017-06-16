using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using log4net;
using System.Reflection;

namespace EZRAC.Core.Logging.Log4Net
{
    /// <summary>
    /// Represents helper class for error logging using log4net.
    /// </summary>    
    public static class LogHelper
    {
        public static ILogger GetLogger()
        {
            //Get log4net logger
            return new Log4Net(Assembly.GetCallingAssembly().GetName().Name);
        }
    }
}
