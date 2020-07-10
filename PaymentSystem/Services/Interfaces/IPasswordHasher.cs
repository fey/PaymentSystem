namespace PaymentSystem.Services.Interfaces
{
    public interface IPassowrdHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string hash, string password);
    }
}