using PaymentSystem.Model;
using PaymentSystem.Model.Dto.Payments;

namespace PaymentSystem.Services.Interfaces
{
    public interface IPaymentValidator
    {
        CardValidationResults ValidateCard(Card card);
    }
}