using System;
using System.Threading;
using System.Threading.Tasks;
using PaymentSystem.Services.Interfaces;

namespace PaymentSystem.Services.Implementations
{
    public class StubNotifier: INotifier
    {
        private void DoSendNotification(Uri target, String message)
        {
            Console.WriteLine($"Sending notification to {target}...");
            Thread.Sleep(10000);
            Console.WriteLine($"Notification to {target} was successfully sent.");
        }

        public Task SendAsyncNotification(Uri target, String message)
        {
            if (
                target.Scheme != Uri.UriSchemeHttp &&
                target.Scheme != Uri.UriSchemeHttps
            )
                return null;
            return new Task(() => DoSendNotification(target, message));
        }
    }
}