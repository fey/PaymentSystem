using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PaymentSystem.Model.Dto.Payments;

namespace PaymentSystem.Services.Interfaces
{
    public interface IPaymentRepository
    {
        IAsyncEnumerable<PaymentRecord> GetPaymentHistoryAsync(DateTime start, DateTime end);
        Task MakePaymentAsync(Guid sessionId, Card paymentCard);
        Task<Guid> RecordPaymentAsync(PaymentRequest payment);
        Task SessionIsActiveAsync(Guid sessionId);
    }
}