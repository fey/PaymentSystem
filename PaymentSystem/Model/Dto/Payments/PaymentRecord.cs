using System;
using System.ComponentModel.DataAnnotations;

namespace PaymentSystem.Model.Dto.Payments
{
    public class PaymentRecord
    {
        [Required]
        [DataType(DataType.Currency)]
        public double PaymentSum { get; set; }
        [Required]
        public string Purpose { get; set; }
        [DataType(DataType.CreditCard)]
        public string CardNumber { get; set; }
        [DataType(DataType.Date)]
        public DateTime PaymentRequestDateTime { get; set; }
    }
}