using System;
using System.Globalization;
using PaymentSystem.Model;
using PaymentSystem.Model.Dto.Payments;
using Xunit;

namespace PaymentSystem.Services.Implementations.Tests
{
    public class CardValidatorTests
    {
        [Fact]
        public void ShouldNoticeEmptyCardNumber()
        {
            CardValidator validator = new CardValidator();
            CardValidationResults result = validator.ValidateCard(new Card());
            Assert.Equal(CardValidationResults.InvalidNumber, result);
        }

        [Fact]
        public void ShouldNoticeEmptySecurityCode()
        {
            CardValidator validator = new CardValidator();
            CardValidationResults result = validator.ValidateCard(new Card() 
            {
                Number = "421"
            });
            Assert.Equal(CardValidationResults.InvalidSecurityCode, result);
        }

        [Fact]
        public void ShouldNoticeWrongCardRegistrationDate()
        {
            CardValidator validator = new CardValidator();
            CardValidationResults result = validator.ValidateCard(new Card()
            {
                Number = "421",
                SecurityCode = "404",
                RegistrationDate = DateTime.ParseExact("1/21", "d/yy", CultureInfo.InvariantCulture)
            });
            Assert.Equal(CardValidationResults.Expired, result);
        }

        [Fact]
        public void ShouldNoticeExpiredCard()
        {
            CardValidator validator = new CardValidator();
            CardValidationResults result = validator.ValidateCard(new Card()
            {
                Number = "421",
                SecurityCode = "404",
                ExpirationDate = DateTime.ParseExact("1/19", "d/yy", CultureInfo.InvariantCulture)
            });
            Assert.Equal(CardValidationResults.Expired, result);
        }

        [Fact]
        public void ShouldNoticeInvalidCardNumber()
        {
            CardValidator validator = new CardValidator();
            CardValidationResults result = validator.ValidateCard(new Card()
            {
                Number = "4561261212345464",
                SecurityCode = "404"
            });
            Assert.Equal(CardValidationResults.InvalidNumber, result);
        }

        [Fact]
        public void ShouldNoticeNaNCardNumber()
        {
            CardValidator validator = new CardValidator();
            CardValidationResults result = validator.ValidateCard(new Card()
            {
                Number = "4a561261212345464",
                SecurityCode = "404"
            });
            Assert.Equal(CardValidationResults.InvalidNumber, result);
        }

        [Fact]
        public void ShouldAcceptValidCardNumber()
        {
            CardValidator validator = new CardValidator();
            CardValidationResults result = validator.ValidateCard(new Card()
            {
                Number = "4561261212345467",
                SecurityCode = "404"
            });
            Assert.Equal(CardValidationResults.Valid, result);
        }

        [Fact]
        public void ShouldAcceptCompletelyValidCard()
        {
            CardValidator validator = new CardValidator();
            CardValidationResults result = validator.ValidateCard(new Card()
            {
                Number = "4561261212345467",
                SecurityCode = "404",
                RegistrationDate = DateTime.ParseExact("1/19", "d/yy", CultureInfo.InvariantCulture),
                ExpirationDate = DateTime.ParseExact("1/21", "d/yy", CultureInfo.InvariantCulture)
            });
            Assert.Equal(CardValidationResults.Valid, result);
        }
    }
}