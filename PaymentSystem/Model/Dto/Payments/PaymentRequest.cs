using System.ComponentModel.DataAnnotations;

namespace PaymentSystem.Model.Dto.Payments
{
    public class PaymentRequest
    {
        [DataType(DataType.Currency)]
        [Required(ErrorMessage = "Payment sum not defined")]
        public decimal Sum { get; set; }
        [Required(ErrorMessage = "Purpose not defined")]
        public string Purpose { get; set; }
    }
}