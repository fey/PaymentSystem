using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PaymentSystem.Model.Dto.Payments;

namespace PaymentSystem.Services.Interfaces
{
    public interface IPaymentRepository
    {
        IEnumerable<PaymentRecord> GetPaymentHistory(DateTime start, DateTime end);
        IAsyncEnumerable<PaymentRecord> GetPaymentHistoryAsync(DateTime start, DateTime end);
        bool MakePayment(Guid sessionId, Card paymentCard);
        Task<bool> MakePaymentAsync(Guid sessionId, Card paymentCard);
        Guid RecordPayment(PaymentRequest payment);
        Task<Guid> RecordPaymentAsync(PaymentRequest payment);
        bool SessionIsActive(Guid sessionId);
        Task<bool> SessionIsActiveAsync(Guid sessionId);
    }
}