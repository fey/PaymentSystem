namespace PaymentSystem.Services.Interfaces
{
    public class BCryptPasswordHasher : IPassowrdHasher
    {
        public string HashPassword(string password) =>
            BCrypt.Net.BCrypt.HashPassword(password);

        public bool VerifyPassword(string hash, string password) =>
            BCrypt.Net.BCrypt.Verify(password, hash);
    }
}