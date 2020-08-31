using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PaymentSystem.Database;
using PaymentSystem.Model.Dto.Auth;
using PaymentSystem.Model.Stored;
using PaymentSystem.Services.Exceptions;
using PaymentSystem.Services.Interfaces;

namespace PaymentSystem.Services.Implementations
{
    public class DbUserRepository : IUserRepository
    {
        private readonly IPassowrdHasher _passwordHasher;
        private readonly UserContext _context;

        public DbUserRepository(IPassowrdHasher passwordHasher, UserContext context)
        {
            _passwordHasher = passwordHasher;
            _context = context;
        }

        public async Task AddUserAsync(RegisterModel newUser)
        {
            if (await _context.Users.AnyAsync(user => user.Login.Equals(newUser.Username)))
                throw new RegistrationException();
            await _context.Users.AddAsync(new User() 
                { 
                    Id = Guid.NewGuid(),
                    Login = newUser.Username,
                    HashedPassword  = _passwordHasher.HashPassword(newUser.Password)
                });
            if ((await _context.SaveChangesAsync()) == 0)
                throw new RegistrationException();
        }

        public async Task VerifyCredentialsAsync(LoginModel credentials)
        {
            string password = await _context.Users
                .Where(user => user.Login.Equals(credentials.Login))
                .Select(user => user.HashedPassword)
                .FirstOrDefaultAsync();
            if (password == null || 
                !_passwordHasher.VerifyPassword(password, credentials.Password))
                throw new AuthenticationException();
        }
    }
}