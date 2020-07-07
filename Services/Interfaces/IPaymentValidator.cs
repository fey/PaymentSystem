using PaymentSystem.Model;

namespace PaymentSystem.Services.Interfaces
{
    public interface IPaymentValidator
    {
        CardValidationResults ValidateCard(Card card);
    }
}