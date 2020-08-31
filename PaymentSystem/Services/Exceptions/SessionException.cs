namespace PaymentSystem.Services.Exceptions
{
    public class SessionException: NotFoundException 
    {
        public SessionException():base("Session is not active or payment for this session was already made") {}
    }
}