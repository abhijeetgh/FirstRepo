using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.Reflection;
using System.Globalization;

namespace EZRAC.Core.Logging.Log4Net
{
    public class Log4Net : ILogger
    {
        private ILog log;

        //public Log4Net(string loggerName)
        public Log4Net()        
        {
            log4net.Config.XmlConfigurator.Configure();
            //var loggerName = Assembly.GetEntryAssembly().GetName().Name;
            log = LogManager.GetLogger("EZRAC.Core.Logger" + Convert.ToString(AppDomain.CurrentDomain.Id, CultureInfo.InvariantCulture));//LogManager.GetLogger(loggerName);
        }

        public void LogInformation(string customMessage)
        {
            log.Info(customMessage);
        }

        public void LogInformation(string message, string sourceName, string methodName)
        {
            string SourceAndMethodName = "Information message from class: {0} and method: {1}. Exception Details:{2}";
            log.Info(string.Format(SourceAndMethodName, sourceName, methodName, message));
        }        

        public void LogWarning(string customMessage)
        {
            log.Warn(customMessage);
        }

        public void LogWarning(string warningMessage, string sourceName, string methodName)
        {
            string SourceAndMethodName = "Warning message from class: {0} and method: {1}. Exception Details:{2}";
            log.Warn(string.Format(SourceAndMethodName, sourceName, methodName, warningMessage));
        }

        public void LogDebug(Exception ex, string customMessage)
        {
            log.Debug(customMessage, ex); ;
        }

        public void LogDebug(string customMessage)
        {
            log.Debug(customMessage);
        }

        public void LogDebug(string debugMessage, string sourceName, string methodName)
        {
            string SourceAndMethodName = "Exception occurred in class: {0} and method: {1}. Exception Details:{2}";
            log.Debug(string.Format(SourceAndMethodName, sourceName, methodName, debugMessage));
        }

        public void LogError(Exception ex, string customMessage)
        {
            log.Error(customMessage, ex); ;
        }

        public void LogError(string customMessage)
        {
            log.Error(customMessage);
        }

        public void LogError(string errorMessage, string sourceName, string methodName)
        {
            string SourceAndMethodName = "Exception occurred in class: {0} and method: {1}. Exception Details:{2}";
            log.Error(string.Format(SourceAndMethodName, sourceName, methodName, errorMessage));
        }
    }
}
