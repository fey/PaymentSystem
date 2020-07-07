using System;
using PaymentSystem.Model;

namespace PaymentSystem.Services.Interfaces
{
    public interface IPaymentRepository
    {
        bool MakePayment(Guid sessionId, Card paymentCard, string source);
        Guid RecordPayment(Payment payment);
        bool SessionIsActive(Guid sessionId);
    }
}