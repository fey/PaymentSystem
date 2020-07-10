using System.ComponentModel.DataAnnotations;

namespace PaymentSystem.Model.Common
{
    public class Payment
    {
        [DataType(DataType.Currency)]
        [Required(ErrorMessage = "Payment sum not defined")]
        public decimal PaymentSum { get; set; }
        [Required(ErrorMessage = "Purpose not defined")]
        public string Purpose { get; set; }
    }
}