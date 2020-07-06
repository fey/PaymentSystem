using System;
using System.Threading.Tasks;
using PaymentSystem.Services.Interfaces;

namespace PaymentSystem.Services.Implementations
{
    class StubNotifier : INotifier
    {
        public Task SendNotification(Uri target)
        {
            throw new NotImplementedException();
        }
    }
}