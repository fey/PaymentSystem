using System;

namespace PaymentSystem.Services.Exceptions
{
    public abstract class ServiceException: Exception 
    {
        public ServiceException(string message):base(message) {}
        public abstract int Code {get;}
    }
}