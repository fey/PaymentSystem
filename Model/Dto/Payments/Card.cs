using System.ComponentModel.DataAnnotations;

namespace PaymentSystem.Model.Dto.Payments
{
    public class Card
    {
        [DataType(DataType.CreditCard)]
        public string Number { get; set; }
        public string SecurityCode { get; set; }
        public Date RegistrationDate { get; set; }
        public Date ExpirationDate { get; set; }
    }
}