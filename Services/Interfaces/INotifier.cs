using System;

namespace PaymentSystem.Services.Interfaces
{
    public interface INotifier<T>
    {
        void SendAsyncNotification(Uri target, T message);
    }
}