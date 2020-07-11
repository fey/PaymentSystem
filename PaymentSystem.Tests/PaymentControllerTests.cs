using System;
using AutoFixture;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Mvc;
using PaymentSystem.Model.Common;
using PaymentSystem.Model.Dto.Payments;
using PaymentSystem.Services.Implementations;
using PaymentSystem.Services.Interfaces;
using Xunit;

namespace PaymentSystem.Controllers.Tests
{
    public class PaymentControllerTests
    {
        class AutoControllerAttribute: AutoDataAttribute
        {
            public AutoControllerAttribute()
                : base(() =>
                {
                    Fixture fixture = new Fixture();
                    fixture.Register<IPaymentRepository>(
                        () => new StubPaymentRepository());
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
        public void ShouldGivePaymentSessionId(
            [NoAutoProperties]PaymentController controller,
            Payment payment
        ) => Assert.IsType<ActionResult<Guid>>(
            controller.GetPaymentSession(payment)
        );

        [Theory, AutoController]
        public async void ShouldNotMakePaymentForInvalidSessionId(
            [NoAutoProperties]PaymentController controller,
            Card card
        ) => Assert.IsType<ForbidResult>(
            await controller.InitiatePayment(card, Guid.Empty, null)
        );

        [Theory, AutoController]
        public async void ShouldNotMakePaymentForInvalidCard(
            [NoAutoProperties]PaymentController controller,
            Payment payment
        )
        {
            Card invalidCard = new Card()
            {
                Number = "4a561261212345464",
                SecurityCode = "404"
            };
            ActionResult<Guid> result = controller.GetPaymentSession(payment);
            Assert.IsType<BadRequestObjectResult>(
                await controller.InitiatePayment(invalidCard, result.Value, null)
            );
        }

        [Theory, AutoController]
        public async void ShouldMakePaymentForValidCard(
            [NoAutoProperties]PaymentController controller,
            Payment payment
        )
        {
            Card validCard = new Card() {
                Number = "4561261212345467",
                SecurityCode = "404"
            };
            ActionResult<Guid> result = controller.GetPaymentSession(payment);
            Assert.IsType<OkResult>(
                await controller.InitiatePayment(validCard, result.Value, null)
            );
        }
    }
}
