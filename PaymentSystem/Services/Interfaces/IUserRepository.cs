using System.Threading.Tasks;
using PaymentSystem.Model.Dto.Auth;

namespace PaymentSystem.Services.Interfaces
{
    public interface IUserRepository
    {
        public Task AddUserAsync(RegisterModel newUser);
        public Task VerifyCredentialsAsync(LoginModel credentials);
    }
}