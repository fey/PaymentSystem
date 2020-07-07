using System;

namespace PaymentSystem.Model
{
    public class SessionDetails
    {
        public Payment AssociatedPayment { get; set; }
        public string Callback { get; set; }
        public string CardNumber { get; set; }
        public DateTime ExpirationDateTime { get; set; }
        public bool PaymentWasMade => !String.IsNullOrWhiteSpace(CardNumber);
    }
}