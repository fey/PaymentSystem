using System;
using System.Collections.Generic;
using System.Threading;
using AutoFixture.Xunit2;
using Microsoft.EntityFrameworkCore;
using PaymentSystem.Database;
using PaymentSystem.Model.Dto.Payments;
using Xunit;

namespace PaymentSystem.Services.Implementations.Tests
{
    public class DbPaymentRepositoryTests
    {
        private DbContextOptions<PaymentContext> optionsFactory =>
            new DbContextOptionsBuilder<PaymentContext>()
                .UseLazyLoadingProxies()
                .UseInMemoryDatabase(databaseName: $"Payments{Guid.NewGuid()}")
                .Options;

        [Theory, AutoData]
        public void ShouldRecordPayment(PaymentRequest payment)
        {
            using (var context = new PaymentContext(optionsFactory))
                Assert.IsType<Guid>(new DbPaymentRepository(context).RecordPayment(payment));
        }
        

        [Theory, AutoData]
        public void ShouldRememberSession(PaymentRequest payment)
        {
            using (var context = new PaymentContext(optionsFactory))
            {
                DbPaymentRepository repository = new DbPaymentRepository(context);
                Assert.True(repository.SessionIsActive(repository.RecordPayment(payment)));
            }
        }

        [Theory, AutoData]
        public void ShouldNotConsiderExpiredSessionActive(PaymentRequest payment)
        {
            using (var context = new PaymentContext(optionsFactory))
            {
                DbPaymentRepository repository = new DbPaymentRepository(context);
                repository.SessionExpirationTimeSpan = TimeSpan.FromSeconds(1);
                Guid sessionId = repository.RecordPayment(payment);
                Thread.Sleep(2 * 1000);
                Assert.False(repository.SessionIsActive(sessionId));
            }
        }

        [Fact]
        public void ShouldNotAcceptAnyGuidAsActiveSessionId()
        {
            using (var context = new PaymentContext(optionsFactory))
            {
                DbPaymentRepository repository = new DbPaymentRepository(context);
                Assert.False(repository.SessionIsActive(Guid.Empty));
            }
        }

        [Theory, AutoData]
        public void ShouldSuccessfullyMakePaymentForActiveSession(
            PaymentRequest payment, Card paymentCard
        )
        {
            using (var context = new PaymentContext(optionsFactory))
            {
                DbPaymentRepository repository = new DbPaymentRepository(context);
                Guid sessionId = repository.RecordPayment(payment);
                Assert.True(repository.MakePayment(sessionId, paymentCard));
            }
        }
        
        [Theory, AutoData]
        public void ShouldNotMakePaymentForInactiveSession(Card paymentCard)
        {
            using (var context = new PaymentContext(optionsFactory))
            {
                DbPaymentRepository repository = new DbPaymentRepository(context);
                Assert.False(repository.MakePayment(Guid.Empty, paymentCard));
            }
        } 
        
        [Theory, AutoData]
        public void ShouldNotMakePaymentForExpiredSession(
            PaymentRequest payment, Card paymentCard
        ) 
        {
            using (var context = new PaymentContext(optionsFactory))
            {
                DbPaymentRepository repository = new DbPaymentRepository(context);
                repository.SessionExpirationTimeSpan = TimeSpan.FromSeconds(1);
                Guid sessionId = repository.RecordPayment(payment);
                Thread.Sleep(2 * 1000);
                Assert.False(repository.MakePayment(sessionId, paymentCard));
            }
        }

        [Theory, AutoData]
        public void ShouldNotRepeatPayment(
            PaymentRequest payment, Card paymentCard
        )
        {
            using (var context = new PaymentContext(optionsFactory))
            {
                DbPaymentRepository repository = new DbPaymentRepository(context);
                Guid sessionId = repository.RecordPayment(payment);
                repository.MakePayment(sessionId, paymentCard);
                Assert.False(repository.MakePayment(sessionId, paymentCard));
            }
        }

        [Theory, AutoData]
        public void ShouldNotListUnfinishedPayments(PaymentRequest payment)
        {
            using (var context = new PaymentContext(optionsFactory))
            {
                DbPaymentRepository repository = new DbPaymentRepository(context);
                repository.SessionExpirationTimeSpan = TimeSpan.FromSeconds(1);
                Guid sessionId = repository.RecordPayment(payment);
                Thread.Sleep(2 * 1000);
                Assert.False(repository.SessionIsActive(sessionId));
            }
        }

        [Theory, AutoData]
        public void ShouldListPaymentsRequestedInDatePeriod(PaymentRequest payment)
        {
            using (var context = new PaymentContext(optionsFactory))
            {
                DbPaymentRepository repository = new DbPaymentRepository(context);
                IEnumerable<PaymentRecord> payments = repository.GetPaymentHistory(
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
        }

        [Theory, AutoData]
        public void ShouldNotListPaymentOutOfDatePeriod(PaymentRequest payment)
        {
            using (var context = new PaymentContext(optionsFactory))
            {
                DbPaymentRepository repository = new DbPaymentRepository(context);
                IEnumerable<PaymentRecord> payments = repository.GetPaymentHistory(
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
}