using System;
using PaymentSystem.Model;
using PaymentSystem.Model.Dto.Payments;
using PaymentSystem.Services.Interfaces;

namespace PaymentSystem.Services.Implementations
{
    public class CardValidator: ICardValidator
    {
        private CardValidationResults SimplifiedLuhnAlgorithm(string cardNumber)
        {
            int parity = cardNumber.Length % 2, sum = 0;
            for (int i = 0; i < cardNumber.Length; i++)
                if (Char.IsDigit(cardNumber[i]))
                {
                    int digit = (int)Char.GetNumericValue(cardNumber[i]),
                        sumPart = i % 2 == parity ? digit * 2 : digit;
                    if (sumPart > 9)
                        sumPart -= 9;
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
            if (String.IsNullOrWhiteSpace(card.SecurityCode))
                return CardValidationResults.InvalidSecurityCode;
            if (
                card.RegistrationDate.HasValue && 
                card.RegistrationDate.Value.Date > DateTime.Today)
                return CardValidationResults.Expired;
            if (
                card.ExpirationDate.HasValue &&
                card.ExpirationDate.Value.Date < DateTime.Today)
                return CardValidationResults.Expired;
            return SimplifiedLuhnAlgorithm(card.Number);
        }
    }
}