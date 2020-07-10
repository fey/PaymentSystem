using System;
using System.Collections.Generic;
using PaymentSystem.Model.Common;
using PaymentSystem.Model.Dto.Payments;

namespace PaymentSystem.Services.Interfaces
{
    public interface IPaymentRepository
    {
        List<PaymentRecord> GetPaymentHistory(DateTime start, DateTime end);
        bool MakePayment(Guid sessionId, Card paymentCard, string source);
        Guid RecordPayment(Payment payment);
        bool SessionIsActive(Guid sessionId);
    }
}