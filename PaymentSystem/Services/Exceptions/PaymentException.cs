namespace PaymentSystem.Services.Exceptions
{
    public class PaymentException: ServiceException 
    {
        public PaymentException():base("Something went wrong while making payment") {}
        public override int Code => 800;
    }
}