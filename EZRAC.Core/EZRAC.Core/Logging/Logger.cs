using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.Core.Logging
{
    public static class Logger
    {
        private static readonly ILogger _logger;
        private static readonly CustomSchedular _schedular;
        private const int _asyncLogWriterThreadCount = 1;

        static Logger()
        {
            _schedular = new CustomSchedular(_asyncLogWriterThreadCount);
            _logger = ServiceLocator.Current.GetInstance<ILogger>();
        }

        #region LogError methods

        public static void LogError(Exception exception, string customMessage)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");
            if (customMessage == null)
                throw new ArgumentNullException("message");

            _logger.LogError(exception, customMessage);
        }

        public static void LogErrorAsync(Exception exception, string customMessage)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            new Task(() =>
            {
                LogError(exception, customMessage);
            }).Start(_schedular);
        }

        public static void LogError(string customMessage)
        {
            if (customMessage == null)
                throw new ArgumentNullException("customMessage");

            _logger.LogError(customMessage);
        }

        public static void LogErrorAsync(string customMessage)
        {
            if (customMessage == null)
                throw new ArgumentNullException("customMessage");

            new Task(() =>
            {
                LogError(customMessage);
            }).Start(_schedular);
        }

        public static void LogErrorAsync(string errorMessage, string sourceName, string methodName)
        {
            if (errorMessage == null)
                throw new ArgumentNullException("message");
            if (sourceName == null)
                throw new ArgumentNullException("sourceName");

            new Task(() =>
            {
                LogError(errorMessage, sourceName, methodName);
            }).Start(_schedular);
        }

        public static void LogError(string errorMessage, string sourceName, string methodName)
        {
            if (errorMessage == null)
                throw new ArgumentNullException("message");
            if (sourceName == null)
                throw new ArgumentNullException("sourceName");

            _logger.LogError(errorMessage, sourceName, methodName);
        }

        #endregion

        #region LogWarning methods

        public static void LogWarning(string customMessage)
        {
            if (customMessage == null)
                throw new ArgumentNullException("message");

            _logger.LogWarning(customMessage);
        }

        public static void LogWarningAsync(string customMessage)
        {
            if (customMessage == null)
                throw new ArgumentNullException("message");

            new Task(() =>
            {
                LogWarning(customMessage);
            }).Start(_schedular);
        }

        public static void LogWarningAsync(string warningMessage, string sourceName, string methodName)
        {
            if (warningMessage == null)
                throw new ArgumentNullException("message");

            new Task(() =>
            {
                LogWarning(warningMessage, sourceName, methodName);
            }).Start(_schedular);
        }

        public static void LogWarning(string warningMessage, string sourceName, string methodName)
        {
            if (warningMessage == null)
                throw new ArgumentNullException("message");

            _logger.LogWarning(warningMessage, sourceName, methodName);
        }

        #endregion

        #region LogInformation methods

        /// <summary>
        /// Logs an informational message with the given message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="categories">The category names used to route the log entry.</param>
        /// <exception cref="ArgumentNullException">message is null</exception>
        public static void LogInformation(string customMessage)
        {
            if (customMessage == null)
                throw new ArgumentNullException("message");

            _logger.LogInformation(customMessage);
        }


        public static void LogInformationAsync(string customMessage)
        {
            if (customMessage == null)
                throw new ArgumentNullException("message");

            new Task(() =>
            {
                LogWarning(customMessage);
            }).Start(_schedular);
        }
        /// <summary>
        /// Logs an informational message with the .
        /// </summary>        
        /// <param name="categories">The category names used to route the log entry.</param>
        /// <exception cref="ArgumentNullException">exception is null</exception>
        public static void LogInformation(string message, string sourceName, string methodName)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            _logger.LogInformation(message, sourceName, methodName);
        }


        public static void LogInformationAsync(string message, string sourceName, string methodName)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            new Task(() =>
            {
                LogWarning(message, sourceName, methodName);
            }).Start(_schedular);
        }

        #endregion

        #region LogDebug methods

        /// <summary>
        /// Logs a debug message with the given message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="categories">The category names used to route the log entry.</param>
        /// <exception cref="ArgumentNullException">message is null</exception>
        public static void LogDebug(string customMessage)
        {
            if (customMessage == null)
                throw new ArgumentNullException("customMessage");

            _logger.LogDebug(customMessage);
        }

        public static void LogDebugAsync(string customMessage)
        {
            if (customMessage == null)
                throw new ArgumentNullException("customMessage");

            new Task(() =>
            {
                LogWarning(customMessage);
            }).Start(_schedular);
        }

        /// <summary>
        /// Logs a debug message with the given exception.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="categories">The category names used to route the log entry.</param>
        /// <exception cref="ArgumentNullException">exception is null</exception>
        public static void LogDebug(string debugMessage, string sourceName, string methodName)
        {
            if (debugMessage == null)
                throw new ArgumentNullException("message");
            _logger.LogDebug(debugMessage, sourceName, methodName);
        }


        public static void LogDebugAsync(string debugMessage, string sourceName, string methodName)
        {
            if (debugMessage == null)
                throw new ArgumentNullException("debugMessage");

            new Task(() =>
            {
                LogWarning(debugMessage, sourceName, methodName);
            }).Start(_schedular);
        }

        #endregion
    }

}
