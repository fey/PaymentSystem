namespace PaymentSystem.Services.Exceptions
{
    public class InvalidSecurityCodeException: ServiceException 
    { 
        public InvalidSecurityCodeException():base("Invalid security code") {}
        public override int Code => 2;
    }
}