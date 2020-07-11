using System;
using System.Collections.Generic;
using System.Threading;
using AutoFixture.Xunit2;
using PaymentSystem.Model.Common;
using PaymentSystem.Model.Dto.Payments;
using Xunit;

namespace PaymentSystem.Services.Implementations.Tests
{
    public class StubPaymentRepositoryTests
    {
        [Theory, AutoData]
        public void ShouldRecordPayment(
            [NoAutoProperties]StubPaymentRepository repository, Payment payment
        ) => Assert.IsType<Guid>(repository.RecordPayment(payment));

        [Theory, AutoData]
        public void ShouldRememberSession(
             [NoAutoProperties]StubPaymentRepository repository, Payment payment
        ) => Assert.True(repository.SessionIsActive(repository.RecordPayment(payment)));

        [Theory, AutoData]
        public void ShouldNotConsiderExpiredSessionActive(
             [NoAutoProperties]StubPaymentRepository repository, Payment payment
        )
        {
            repository.SessionExpirationTimeSpan = TimeSpan.FromSeconds(1);
            Guid sessionId = repository.RecordPayment(payment);
            Thread.Sleep(2 * 1000);
            Assert.False(repository.SessionIsActive(sessionId));
        }

        [Theory, AutoData]
        public void ShouldNotAcceptAnyGuidAsActiveSessionId(
            [NoAutoProperties]StubPaymentRepository repository
        ) => Assert.False(repository.SessionIsActive(Guid.Empty));

        [Theory, AutoData]
        public void ShouldSuccessfullyMakePaymentForActiveSession(
            [NoAutoProperties]StubPaymentRepository repository, Payment payment, Card paymentCard
        )
        {
            Guid sessionId = repository.RecordPayment(payment);
            Assert.True(repository.MakePayment(sessionId, paymentCard, null));
        }
        
        [Theory, AutoData]
        public void ShouldNotMakePaymentForInactiveSession(
            [NoAutoProperties]StubPaymentRepository repository, Card paymentCard
        ) => Assert.False(repository.MakePayment(Guid.Empty, paymentCard, null));
        
        [Theory, AutoData]
        public void ShouldNotMakePaymentForExpiredSession(
            [NoAutoProperties]StubPaymentRepository repository, Payment payment, Card paymentCard
        ) 
        {
            repository.SessionExpirationTimeSpan = TimeSpan.FromSeconds(1);
            Guid sessionId = repository.RecordPayment(payment);
            Thread.Sleep(2 * 1000);
            Assert.False(repository.MakePayment(sessionId, paymentCard, null));
        }

        [Theory, AutoData]
        public void ShouldNotRepeatPayment(
            [NoAutoProperties]StubPaymentRepository repository, Payment payment, Card paymentCard
        )
        {
            Guid sessionId = repository.RecordPayment(payment);
            repository.MakePayment(sessionId, paymentCard, null);
            Assert.False(repository.MakePayment(sessionId, paymentCard, null));
        }

        [Theory, AutoData]
        public void ShouldNotListUnfinishedPayments(
             [NoAutoProperties]StubPaymentRepository repository, Payment payment
        )
        {
            repository.SessionExpirationTimeSpan = TimeSpan.FromSeconds(1);
            Guid sessionId = repository.RecordPayment(payment);
            Thread.Sleep(2 * 1000);
            Assert.False(repository.SessionIsActive(sessionId));
        }

        [Theory, AutoData]
        public void ShouldListPaymentsRequestedInDatePeriod(
             [NoAutoProperties]StubPaymentRepository repository, Payment payment
        )
        {
            List<PaymentRecord> payments = repository.GetPaymentHistory(
                DateTime.Today, DateTime.Today
            );
            Assert.Empty(payments);
            Guid id = repository.RecordPayment(payment);
            payments = repository.GetPaymentHistory(
                DateTime.Today, DateTime.Today
            );
            Assert.Collection(payments, item => {
                Assert.Equal(payment.Sum, item.PaymentSum);
                Assert.Equal( payment.Purpose, item.Purpose);
            });
        }

        [Theory, AutoData]
        public void ShouldNotListPaymentOutOfDatePeriod(
             [NoAutoProperties]StubPaymentRepository repository, Payment payment
        )
        {
            List<PaymentRecord> payments = repository.GetPaymentHistory(
                DateTime.Today, DateTime.Today
            );
            Assert.Empty(payments);
            Guid id = repository.RecordPayment(payment);
            payments = repository.GetPaymentHistory(
                DateTime.Today.AddDays(2), DateTime.Today.AddDays(3)
            );
            Assert.Empty(payments);
        }
    }
}