using System;
using Microsoft.Practices.ServiceLocation;

namespace EZRAC.Core.ExceptionHandling
{
    public static class ExceptionManager
    {
        private static readonly IExceptionManager _exceptionManager;

        static ExceptionManager()
        {
            
            _exceptionManager = ServiceLocator.Current.GetInstance<IExceptionManager>();
        }

        public static bool HandleException(Exception exceptionToHandle)
        {
            return _exceptionManager.HandleException(exceptionToHandle);
        }
       
    }
}
