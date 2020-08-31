namespace PaymentSystem.Services.Exceptions
{
    public class NotFoundException: ServiceException 
    {
        public NotFoundException(string message):base(message) {}
        public override int Code => 404;
    }
}