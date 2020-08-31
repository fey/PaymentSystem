using System.ComponentModel.DataAnnotations;

namespace PaymentSystem.Model.Dto.Payments
{
    public class PaymentRequest
    {
        [DataType(DataType.Currency)]
        [Range(1, double.MaxValue)]
        [Required(ErrorMessage = "Payment sum not defined")]
        public double Sum { get; set; }
        [Required(ErrorMessage = "Purpose not defined")]
        public string Purpose { get; set; }
    }
}