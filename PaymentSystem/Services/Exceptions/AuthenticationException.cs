namespace PaymentSystem.Services.Exceptions
{
    public class AuthenticationException: ServiceException 
    {
        public AuthenticationException(): base("Wrong username or password") {}
        public override int Code => 403;
    }
}