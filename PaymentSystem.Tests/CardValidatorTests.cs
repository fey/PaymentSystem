using System;
using AutoFixture.Xunit2;
using PaymentSystem.Model.Dto.Payments;
using PaymentSystem.Services.Exceptions;
using Xunit;

namespace PaymentSystem.Services.Implementations.Tests
{
    public class CardValidatorTests
    {
        [Theory, AutoData]
        public void ShouldNoticeEmptyCardNumber(CardValidator validator) =>
            Assert.Throws<InvalidCardNumberException>(() => validator.ValidateCard(new Card()));

        [Theory, AutoData]
        public void ShouldNoticeEmptySecurityCode(CardValidator validator) =>
            Assert.Throws<InvalidSecurityCodeException>(() => validator.ValidateCard(new Card() 
                {
                    Number = "421"
                }));

        [Theory, AutoData]
        public void ShouldNoticeWrongCardRegistrationDate(CardValidator validator) =>
            Assert.Throws<ExpiredCardException>(() => validator.ValidateCard(new Card()
                {
                    Number = "421",
                    SecurityCode = "404",
                    RegistrationDate = DateTime.ParseExact("1/21", "d/yy", null)
                }));

        [Theory, AutoData]
        public void ShouldNoticeExpiredCard(CardValidator validator) =>
            Assert.Throws<ExpiredCardException>(() => validator.ValidateCard(new Card()
                {
                    Number = "421",
                    SecurityCode = "404",
                    ExpirationDate = DateTime.ParseExact("1/19", "d/yy", null)
                }));

        [Theory, AutoData]
        public void ShouldNoticeInvalidCardNumber(CardValidator validator) =>
            Assert.Throws<InvalidCardNumberException>(() => validator.ValidateCard(new Card()
                {
                    Number = "4561261212345464",
                    SecurityCode = "404"
                }));

        [Theory, AutoData]
        public void ShouldNoticeNaNCardNumber(CardValidator validator) =>
            Assert.Throws<InvalidCardNumberException>(() => validator.ValidateCard(new Card()
                {
                    Number = "4a561261212345464",
                    SecurityCode = "404"
                }));

        [Theory, AutoData]
        public void ShouldAcceptValidCardNumber(CardValidator validator) =>
            validator.ValidateCard(new Card()
            {
                Number = "4561261212345467",
                SecurityCode = "404"
            });

        [Theory, AutoData]
        public void ShouldAcceptCompletelyValidCard(CardValidator validator) =>
            validator.ValidateCard(new Card()
            {
                Number = "4561261212345467",
                SecurityCode = "404",
                RegistrationDate = DateTime.ParseExact("1/19", "d/yy", null),
                ExpirationDate = DateTime.ParseExact("1/21", "d/yy", null)
            });
    }
}