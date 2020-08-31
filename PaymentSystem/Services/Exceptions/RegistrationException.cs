namespace PaymentSystem.Services.Exceptions
{
    public class RegistrationException: ServiceException 
    {
        public RegistrationException(): base("Username is already occupied") {}
        public override int Code => 1001;
    }
}