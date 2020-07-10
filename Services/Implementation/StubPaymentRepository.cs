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
    class StubPaymentRepository : IPaymentRepository
    {
        private readonly ConcurrentDictionary<Guid, PaymentSessionDetails> _payments;

        public StubPaymentRepository()
        {
            _payments = new ConcurrentDictionary<Guid, PaymentSessionDetails>();
        }

        public List<PaymentRecord> GetPaymentHistory(DateTime start, DateTime end)
        {
            return _payments.Values.Where(sessionDetails => 
                sessionDetails.PaymentDateTime.Date >= start.Date &&
                sessionDetails.PaymentDateTime.Date <= end.Date
            ).Select(sessionDetails => new PaymentRecord(){
                PaymentSum = sessionDetails.AssociatedPayment.PaymentSum,
                Purpose = sessionDetails.AssociatedPayment.Purpose,
                CardNumber = sessionDetails.CardNumber,
                PaymentDateTime = sessionDetails.PaymentDateTime
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
            _payments[sessionId].PaymentDateTime = DateTime.Now;
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
                        ExpirationDateTime = DateTime.Now.AddMinutes(10)
                    });
            }
            while (!paymentSuccessfullyRecorded);
            return newSessionId;
        }

        public bool SessionIsActive(Guid sessionId) => 
            _payments.ContainsKey(sessionId) &&
            !_payments[sessionId].PaymentWasMade &&
            DateTime.Today < _payments[sessionId].ExpirationDateTime;
    }
}