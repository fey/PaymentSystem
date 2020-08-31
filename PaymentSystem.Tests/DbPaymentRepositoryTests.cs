using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AutoFixture.Xunit2;
using Microsoft.EntityFrameworkCore;
using PaymentSystem.Database;
using PaymentSystem.Model.Dto.Payments;
using PaymentSystem.Services.Exceptions;
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
        public async void ShouldRecordPayment(PaymentRequest payment)
        {
            using (var context = new PaymentContext(optionsFactory))
                Assert.IsType<Guid>(await new DbPaymentRepository(context).RecordPaymentAsync(payment));
        }
        

        [Theory, AutoData]
        public async void ShouldRememberSession(PaymentRequest payment)
        {
            using (var context = new PaymentContext(optionsFactory))
            {
                DbPaymentRepository repository = new DbPaymentRepository(context);
                await repository.SessionIsActiveAsync(await repository.RecordPaymentAsync(payment));
            }
        }

        [Theory, AutoData]
        public async void ShouldNotConsiderExpiredSessionActive(PaymentRequest payment)
        {
            using (var context = new PaymentContext(optionsFactory))
            {
                DbPaymentRepository repository = new DbPaymentRepository(context);
                repository.SessionExpirationTimeSpan = TimeSpan.FromSeconds(1);
                Guid sessionId = await repository.RecordPaymentAsync(payment);
                Thread.Sleep(2 * 1000);
                await Assert.ThrowsAsync<SessionException>(() => repository.SessionIsActiveAsync(sessionId));
            }
        }

        [Fact]
        public async void ShouldNotAcceptAnyGuidAsActiveSessionId()
        {
            using (var context = new PaymentContext(optionsFactory))
            {
                DbPaymentRepository repository = new DbPaymentRepository(context);
                await Assert.ThrowsAsync<SessionException>(() => repository.SessionIsActiveAsync(Guid.Empty));
            }
        }

        [Theory, AutoData]
        public async void ShouldSuccessfullyMakePaymentForActiveSession(
            PaymentRequest payment, Card paymentCard
        )
        {
            using (var context = new PaymentContext(optionsFactory))
            {
                DbPaymentRepository repository = new DbPaymentRepository(context);
                Guid sessionId = await repository.RecordPaymentAsync(payment);
                await repository.MakePaymentAsync(sessionId, paymentCard);
            }
        }
        
        [Theory, AutoData]
        public async void ShouldNotMakePaymentForInactiveSession(Card paymentCard)
        {
            using (var context = new PaymentContext(optionsFactory))
            {
                DbPaymentRepository repository = new DbPaymentRepository(context);
                await Assert.ThrowsAsync<PaymentException>(() => repository.MakePaymentAsync(Guid.Empty, paymentCard));
            }
        } 
        
        [Theory, AutoData]
        public async void ShouldNotMakePaymentForExpiredSession(
            PaymentRequest payment, Card paymentCard
        ) 
        {
            using (var context = new PaymentContext(optionsFactory))
            {
                DbPaymentRepository repository = new DbPaymentRepository(context);
                repository.SessionExpirationTimeSpan = TimeSpan.FromSeconds(1);
                Guid sessionId = await repository.RecordPaymentAsync(payment);
                Thread.Sleep(2 * 1000);
                await Assert.ThrowsAsync<PaymentException>(() =>  repository.MakePaymentAsync(sessionId, paymentCard));
            }
        }

        [Theory, AutoData]
        public async void ShouldNotRepeatPayment(
            PaymentRequest payment, Card paymentCard
        )
        {
            using (var context = new PaymentContext(optionsFactory))
            {
                DbPaymentRepository repository = new DbPaymentRepository(context);
                Guid sessionId = await repository.RecordPaymentAsync(payment);
                await repository.MakePaymentAsync(sessionId, paymentCard);
                await Assert.ThrowsAsync<PaymentException>(() => repository.MakePaymentAsync(sessionId, paymentCard));
            }
        }

        [Theory, AutoData]
        public async void ShouldNotListUnfinishedPayments(PaymentRequest payment)
        {
            using (var context = new PaymentContext(optionsFactory))
            {
                DbPaymentRepository repository = new DbPaymentRepository(context);
                repository.SessionExpirationTimeSpan = TimeSpan.FromSeconds(1);
                Guid sessionId = await repository.RecordPaymentAsync(payment);
                Thread.Sleep(2 * 1000);
                await Assert.ThrowsAsync<SessionException>(() => repository.SessionIsActiveAsync(sessionId));
            }
        }

        [Theory, AutoData]
        public async void ShouldListPaymentsRequestedInDatePeriod(PaymentRequest payment)
        {
            using (var context = new PaymentContext(optionsFactory))
            {
                DbPaymentRepository repository = new DbPaymentRepository(context);
                IEnumerable<PaymentRecord> payments = repository
                    .GetPaymentHistoryAsync(DateTime.Today, DateTime.Today)
                    .ToEnumerable();
                Assert.Empty(payments);
                Guid id = await repository.RecordPaymentAsync(payment);
                payments = repository
                    .GetPaymentHistoryAsync(DateTime.Today, DateTime.Today)
                    .ToEnumerable();
                Assert.Collection(payments, item => {
                    Assert.Equal(payment.Sum, item.PaymentSum);
                    Assert.Equal( payment.Purpose, item.Purpose);
                });
            }
        }

        [Theory, AutoData]
        public async void ShouldNotListPaymentOutOfDatePeriod(PaymentRequest payment)
        {
            using (var context = new PaymentContext(optionsFactory))
            {
                DbPaymentRepository repository = new DbPaymentRepository(context);
                IEnumerable<PaymentRecord> payments = repository.GetPaymentHistoryAsync(
                    DateTime.Today, DateTime.Today
                ).ToEnumerable();
                Assert.Empty(payments);
                Guid id = await repository.RecordPaymentAsync(payment);
                payments = repository.GetPaymentHistoryAsync(
                    DateTime.Today.AddDays(2), DateTime.Today.AddDays(3)
                ).ToEnumerable();
                Assert.Empty(payments);
            }
        }
    }
}