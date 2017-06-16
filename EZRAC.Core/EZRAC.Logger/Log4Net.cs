using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.Reflection;

namespace EZRAC.Core.Logging.Log4Net
{
    public class Log4Net : ILogger
    {
        private ILog log;

        public Log4Net(string loggerName)        
        {
            log4net.Config.XmlConfigurator.Configure();
            //var loggerName = Assembly.GetEntryAssembly().GetName().Name;
            log = LogManager.GetLogger(loggerName);
        }

        public void Log(Exception ex, string customMessage)
        {
            log.Error(customMessage, ex); ;
        }

        public void Log(string customMessage)
        {
            log.Fatal(customMessage);
        }

        public void LogError(string message, params string[] categories)
        {            
           // log.Error(customMessage, ex); ;
        }

        public void LogError(Exception exception, params string[] categories)
        {
            log.Error(categories, exception); ;
        }

        public void LogError(string message, Exception exception, params string[] categories)
        {
            throw new NotImplementedException();
        }

        public void LogError(Exception exception, Dictionary<string, object> properties, params string[] categories)
        {
            throw new NotImplementedException();
        }

        public void LogWarning(string message, params string[] categories)
        {
            throw new NotImplementedException();
        }

        public void LogWarning(Exception exception, params string[] categories)
        {
            throw new NotImplementedException();
        }

        public void LogWarning(string message, Exception exception, params string[] categories)
        {
            throw new NotImplementedException();
        }

        public void LogInformation(string message, params string[] categories)
        {
            throw new NotImplementedException();
        }

        public void LogInformation(Exception exception, params string[] categories)
        {
            throw new NotImplementedException();
        }

        public void LogInformation(string message, Exception exception, params string[] categories)
        {
            throw new NotImplementedException();
        }

        public void LogDebug(string message, params string[] categories)
        {
            throw new NotImplementedException();
        }

        public void LogDebug(Exception exception, params string[] categories)
        {
            throw new NotImplementedException();
        }

        public void LogDebug(string message, Exception exception, params string[] categories)
        {
            throw new NotImplementedException();
        }
    }
}
