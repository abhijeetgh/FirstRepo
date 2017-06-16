using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.Core.Logging
{
    public interface ILogger
    {
        #region LogError methods       

        void LogError(Exception ex, string customMessage);
        void LogError(string customMessage);
        void LogError(string errorMessage, string sourceName, string methodName);

        #endregion

        #region LogWarning methods        

        void LogWarning(string customMessage);
        void LogWarning(string warningMessage, string sourceName, string methodName);

        #endregion

        #region LogInformation methods        

        void LogInformation(string customMessage);
        void LogInformation(string message, string sourceName, string methodName);

        #endregion

        #region LogDebug methods
        void LogDebug(string customMessage);
        void LogDebug(string debugMessage, string sourceName, string methodName);
        #endregion
    }
}
