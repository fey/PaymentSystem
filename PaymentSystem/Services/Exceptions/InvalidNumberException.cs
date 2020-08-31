namespace PaymentSystem.Services.Exceptions
{
    public class InvalidCardNumberException: ServiceException 
    {
        public InvalidCardNumberException():base("Invalid card number") {} 
        public override int Code => 1;
    }
}