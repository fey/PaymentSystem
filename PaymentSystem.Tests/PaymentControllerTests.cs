using System;
using AutoFixture;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentSystem.Database;
using PaymentSystem.Model.Dto.Payments;
using PaymentSystem.Services.Exceptions;
using PaymentSystem.Services.Implementations;
using PaymentSystem.Services.Interfaces;
using Xunit;

namespace PaymentSystem.Controllers.Tests
{
    public class PaymentControllerTests
    {
        private class AutoControllerAttribute: AutoDataAttribute
        {
            public AutoControllerAttribute()
                : base(() =>
                {
                    Fixture fixture = new Fixture();
                    fixture.Register<IPaymentRepository>(
                        () => 
                        {
                            var options = new DbContextOptionsBuilder<PaymentContext>()
                                .UseLazyLoadingProxies()
                                .UseInMemoryDatabase(databaseName: $"2Payments{Guid.NewGuid()}")
                                .Options;
                            return new DbPaymentRepository(new PaymentContext(options));
                        }
                    );
                    fixture.Customizations.Add(
                        new TypeRelay(
                            typeof(INotifier), 
                            typeof(StubNotifier)
                        )
                    );
                    fixture.Customizations.Add(
                        new TypeRelay(
                            typeof(ICardValidator), 
                            typeof(CardValidator)
                        )
                    );
                    return fixture;
                })
            {
            }
        }

        [Theory, AutoController]
        public async void ShouldGivePaymentSessionId(
            [NoAutoProperties]PaymentController controller,
            PaymentRequest payment
        ) 
        {
            OkObjectResult result = Assert.IsType<OkObjectResult>(
                await controller.GetPaymentSession(payment)
            );
            Assert.IsType<Guid>(result.Value);
        } 

        [Theory, AutoController]
        public async void ShouldNotMakePaymentForInvalidSessionId(
            [NoAutoProperties]PaymentController controller,
            Card card
        ) => await Assert.ThrowsAsync<SessionException>(
            () => controller.InitiatePayment(card, Guid.Empty, null)
        );

        [Theory, AutoController]
        public async void ShouldNotMakePaymentForInvalidCard(
            [NoAutoProperties]PaymentController controller,
            PaymentRequest payment
        )
        {
            Card invalidCard = new Card()
            {
                Number = "4a561261212345464",
                SecurityCode = "404"
            };
            OkObjectResult result = (OkObjectResult)await controller.GetPaymentSession(payment);
            await Assert.ThrowsAsync<InvalidCardNumberException>(() =>
                controller.InitiatePayment(invalidCard, (Guid)result.Value, null)
            );
        }

        [Theory, AutoController]
        public async void ShouldMakePaymentForValidCard(
            [NoAutoProperties]PaymentController controller,
            PaymentRequest payment
        )
        {
            Card validCard = new Card() {
                Number = "4561261212345467",
                SecurityCode = "404"
            };
            OkObjectResult result = (OkObjectResult)await controller.GetPaymentSession(payment);
            await controller.InitiatePayment(validCard, (Guid)result.Value, null);
        }
    }
}
