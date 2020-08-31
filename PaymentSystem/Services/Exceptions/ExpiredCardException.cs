namespace PaymentSystem.Services.Exceptions
{
    public class ExpiredCardException: ServiceException 
    {
        public ExpiredCardException():base("Card is expired") {}
        public override int Code => 3;
    }
}