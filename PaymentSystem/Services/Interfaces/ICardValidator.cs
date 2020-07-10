using PaymentSystem.Model;
using PaymentSystem.Model.Dto.Payments;

namespace PaymentSystem.Services.Interfaces
{
    public interface ICardValidator
    {
        CardValidationResults ValidateCard(Card card);
    }
}