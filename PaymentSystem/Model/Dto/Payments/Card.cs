using System;
using System.ComponentModel.DataAnnotations;

namespace PaymentSystem.Model.Dto.Payments
{
    public class Card
    {
        [DataType(DataType.CreditCard)]
        [Required]
        public String Number { get; set; }
        public String SecurityCode { get; set; }
        [DataType(DataType.Date)]
        public DateTime? RegistrationDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ExpirationDate { get; set; }
    }
}