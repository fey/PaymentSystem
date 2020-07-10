using System;
using System.ComponentModel.DataAnnotations;

namespace PaymentSystem.Model.Dto.Payments
{
    public class Card
    {
        [DataType(DataType.CreditCard)]
        [Required]
        public string Number { get; set; }
        public string SecurityCode { get; set; }
        [DataType(DataType.Date)]
        public DateTime? RegistrationDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ExpirationDate { get; set; }
    }
}