using System.Threading.Tasks;
using PaymentSystem.Model.Dto.Auth;

namespace PaymentSystem.Services.Interfaces
{
    public interface IUserRepository
    {
        public bool AddUser(RegisterModel newUser);
        public Task<bool> AddUserAsync(RegisterModel newUser);
        public bool VerifyCredentials(LoginModel credentials);
        public Task<bool> VerifyCredentialsAsync(LoginModel credentials);
    }
}