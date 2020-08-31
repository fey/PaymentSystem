using PaymentSystem.Model.Dto.Payments;

namespace PaymentSystem.Services.Interfaces
{
    public interface ICardValidator
    {
        void ValidateCard(Card card);
    }
}