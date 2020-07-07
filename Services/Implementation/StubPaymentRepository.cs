using System;
using System.Collections.Concurrent;
using PaymentSystem.Model;
using PaymentSystem.Services.Interfaces;

namespace PaymentSystem.Services.Implementations
{
    class StubPaymentRepository : IPaymentRepository
    {
        private ConcurrentDictionary<Guid, SessionDetails> _payments;

        public bool MakePayment(Guid sessionId, Card paymentCard, string source)
        {
            if (
                !_payments.ContainsKey(sessionId) ||
                _payments[sessionId].PaymentWasMade ||
                DateTime.Today >= _payments[sessionId].ExpirationDateTime
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
                    newSessionId, new SessionDetails()
                    {
                        AssociatedPayment = payment,
                        ExpirationDateTime = DateTime.Today.AddMinutes(10)
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