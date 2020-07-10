using System;
using PaymentSystem.Model;
using PaymentSystem.Model.Dto.Payments;
using PaymentSystem.Services.Interfaces;

namespace PaymentSystem.Services.Implementations
{
    class PaymentValidator: IPaymentValidator
    {
        private CardValidationResults SimplifiedLuhnAlgorithm(string cardNumber)
        {
            int parity = cardNumber.Length % 2, sum = 0;
            for (int i = 0; i < cardNumber.Length; i++)
                if (Char.IsDigit(cardNumber[i]))
                {
                    int digit = (int)Char.GetNumericValue(cardNumber[i]),
                        sumPart = digit % i == parity ? digit : digit * 2;
                    if (sumPart > 9)
                        sumPart /= 2;
                    sum += sumPart;
                }
                else
                    return CardValidationResults.InvalidNumber;
            return sum % 10 == 0 ?
                CardValidationResults.Valid :
                CardValidationResults.InvalidNumber;
        }

        public CardValidationResults ValidateCard(Card card)
        {
            if (String.IsNullOrWhiteSpace(card.Number))
                return CardValidationResults.InvalidNumber;
            if (
                card.RegistrationDate != null && 
                card.RegistrationDate.ToDateTime() > DateTime.Today)
                return CardValidationResults.Expired;
            if (
                card.ExpirationDate != null &&
                card.ExpirationDate.ToDateTime() < DateTime.Today)
                return CardValidationResults.Expired;
            return SimplifiedLuhnAlgorithm(card.Number);
        }
    }
}