using System;

namespace EZRAC.Core.ExceptionHandling
{
    public interface IExceptionManager
    {
        bool HandleException(Exception exceptionToHandle);        
    }
}
