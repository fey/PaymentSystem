using System;
using PaymentSystem.Model.Common;

namespace PaymentSystem.Model.Stored
{
    public class PaymentSessionDetails
    {
        public Payment AssociatedPayment { get; set; }
        public string Callback { get; set; }
        public string CardNumber { get; set; }
        public DateTime? ExpirationDateTime { get; set; }
        public DateTime? PaymentDateTime { get; set; }
        public bool PaymentWasMade => !String.IsNullOrWhiteSpace(CardNumber);
    }
}