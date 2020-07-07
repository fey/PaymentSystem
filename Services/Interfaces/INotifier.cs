using System;
using System.Threading.Tasks;

namespace PaymentSystem.Services.Interfaces
{
    public interface INotifier<T>
    {
        Task SendNotification(Uri target, T message);
    }
}