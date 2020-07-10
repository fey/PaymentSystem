namespace PaymentSystem.Model.Dto.Payments
{
    public class Card
    {
        public string Number { get; set; }
        public string SecurityCode { get; set; }
        public Date RegistrationDate { get; set; }
        public Date ExpirationDate { get; set; }
    }
}