using PaymentSystem.Model.Dto.Auth;

namespace PaymentSystem.Services.Interfaces
{
    public interface IUserRepository
    {
        public bool AddUser(RegisterModel newUser);
        public bool VerifyCredentials(LoginModel credentials);
    }
}