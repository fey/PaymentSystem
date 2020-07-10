using System;

namespace PaymentSystem.Model.Dto.Payments
{
    public class PaymentRecord
    {
        public decimal PaymentSum { get; set; }
        public string Purpose { get; set; }
        public string CardNumber { get; set; }
        public DateTime PaymentDateTime { get; set; }
    }
}