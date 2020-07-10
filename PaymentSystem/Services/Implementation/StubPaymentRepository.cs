using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using PaymentSystem.Model.Common;
using PaymentSystem.Model.Dto.Payments;
using PaymentSystem.Model.Stored;
using PaymentSystem.Services.Interfaces;

namespace PaymentSystem.Services.Implementations
{
    public class StubPaymentRepository : IPaymentRepository
    {
        private readonly ConcurrentDictionary<Guid, PaymentSessionDetails> _payments;
        public TimeSpan SessionExpirationTimeSpan { get; set; }

        public StubPaymentRepository()
        {
            _payments = new ConcurrentDictionary<Guid, PaymentSessionDetails>();
            SessionExpirationTimeSpan = TimeSpan.FromMinutes(10);
        }

        public List<PaymentRecord> GetPaymentHistory(DateTime start, DateTime end)
        {
            return _payments.Values.Where(sessionDetails =>
                sessionDetails.StartDateTime.Date >= start.Date &&
                sessionDetails.StartDateTime.Date <= end.Date
            ).Select(sessionDetails => new PaymentRecord(){
                PaymentSum = sessionDetails.AssociatedPayment.Sum,
                Purpose = sessionDetails.AssociatedPayment.Purpose,
                CardNumber = sessionDetails.CardNumber,
                PaymentRequestDateTime = sessionDetails.StartDateTime
            }).ToList();
        }

        public bool MakePayment(Guid sessionId, Card paymentCard, string source)
        {
            if (
                !_payments.ContainsKey(sessionId) ||
                _payments[sessionId].PaymentWasMade ||
                DateTime.Now >= _payments[sessionId].ExpirationDateTime
            )
                return false;
            _payments[sessionId].CardNumber = paymentCard.Number;
            _payments[sessionId].Callback = source;
            return true;
        }

        public Guid RecordPayment(Payment payment)
        {
            Guid newSessionId = Guid.Empty;
            bool paymentSuccessfullyRecorded = false;
            do 
            {
                newSessionId = Guid.NewGuid();
                paymentSuccessfullyRecorded = _payments.TryAdd(
                    newSessionId, new PaymentSessionDetails()
                    {
                        AssociatedPayment = payment,
                        StartDateTime = DateTime.Now,
                        ExpirationDateTime = DateTime.Now.Add(SessionExpirationTimeSpan)
                    });
            }
            while (!paymentSuccessfullyRecorded);
            return newSessionId;
        }

        public bool SessionIsActive(Guid sessionId) => 
            _payments.ContainsKey(sessionId) &&
            !_payments[sessionId].PaymentWasMade &&
            DateTime.Now < _payments[sessionId].ExpirationDateTime;
    }
}