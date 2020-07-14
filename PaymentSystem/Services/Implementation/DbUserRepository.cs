using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PaymentSystem.Database;
using PaymentSystem.Model.Dto.Auth;
using PaymentSystem.Model.Stored;
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

        public bool AddUser(RegisterModel newUser)
        {
            if (_context.Users.Any(user => user.Login.Equals(newUser.Username)))
                return false;
            _context.Users.Add(new User() 
                { 
                    Id = Guid.NewGuid(),
                    Login = newUser.Username,
                    HashedPassword  = _passwordHasher.HashPassword(newUser.Password)
                });
            return _context.SaveChanges() > 0;
        }

        public async Task<bool> AddUserAsync(RegisterModel newUser)
        {
            if (await _context.Users.AnyAsync(user => user.Login.Equals(newUser.Username)))
                return false;
            await _context.Users.AddAsync(new User() 
                { 
                    Id = Guid.NewGuid(),
                    Login = newUser.Username,
                    HashedPassword  = _passwordHasher.HashPassword(newUser.Password)
                });
            return await _context.SaveChangesAsync() > 0;
        }

        public bool VerifyCredentials(LoginModel credentials)
        {
            string password = _context.Users
                .Where(user => user.Login.Equals(credentials.Login))
                .Select(user => user.HashedPassword)
                .FirstOrDefault();
            return password != null && 
                _passwordHasher.VerifyPassword(password, credentials.Password);
        }

        public async Task<bool> VerifyCredentialsAsync(LoginModel credentials)
        {
            string password = await _context.Users
                .Where(user => user.Login.Equals(credentials.Login))
                .Select(user => user.HashedPassword)
                .FirstOrDefaultAsync();
            return password != null && 
                _passwordHasher.VerifyPassword(password, credentials.Password);
        }
    }
}