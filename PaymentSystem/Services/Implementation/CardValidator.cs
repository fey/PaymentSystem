using System;
using PaymentSystem.Model.Dto.Payments;
using PaymentSystem.Services.Exceptions;
using PaymentSystem.Services.Interfaces;

namespace PaymentSystem.Services.Implementations
{
    public class CardValidator: ICardValidator
    {
        private void SimplifiedLuhnAlgorithm(string cardNumber)
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
                    throw new InvalidCardNumberException();
            
            if (sum % 10 != 0)
                throw new InvalidCardNumberException();
        }

        public void ValidateCard(Card card)
        {
            //BASIC VALIDATION
            if (String.IsNullOrWhiteSpace(card.Number))
                throw new InvalidCardNumberException();
            if (String.IsNullOrWhiteSpace(card.SecurityCode))
                throw new InvalidSecurityCodeException();
            if (card.RegistrationDate.HasValue && 
                card.RegistrationDate.Value.Date > DateTime.Today)
                throw new ExpiredCardException();
            if (card.ExpirationDate.HasValue &&
                card.ExpirationDate.Value.Date < DateTime.Today)
                throw new ExpiredCardException();

            SimplifiedLuhnAlgorithm(card.Number);
        }
    }
}