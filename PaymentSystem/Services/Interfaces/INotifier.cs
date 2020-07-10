using System;
using System.Threading.Tasks;

namespace PaymentSystem.Services.Interfaces
{
    public interface INotifier<T>
    {
        Task SendAsyncNotification(Uri target, T message);
    }
}