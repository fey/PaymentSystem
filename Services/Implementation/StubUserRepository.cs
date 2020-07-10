using System;
using System.Collections.Generic;
using System.Threading;
using PaymentSystem.Model.Dto.Auth;
using PaymentSystem.Model.Stored;
using PaymentSystem.Services.Interfaces;

namespace PaymentSystem.Services.Implementations
{
    public class StubUserRepository : IUserRepository
    {
        private readonly Semaphore _semaphore;
        private readonly List<User> _users;
        private readonly IPassowrdHasher _passwordHasher;

        public StubUserRepository(IPassowrdHasher passwordHasher)
        {
            _semaphore = new Semaphore(0, 1);
            _users = new List<User>();
            _passwordHasher = passwordHasher;
        }

        public bool AddUser(RegisterModel newUser)
        {
            bool success = false;
            _semaphore.WaitOne();
            if (!_users.Exists(user => user.Login.Equals(newUser.Username)))
            {
                _users.Add(new User() 
                { 
                    Id = Guid.NewGuid(),
                    Login = newUser.Username,
                    HashedPassword  = _passwordHasher.HashPassword(newUser.Password)
                });
                success = true;
            }
            _semaphore.Release();
            return success;
        }

        public bool VerifyCredentials(LoginModel credentials)
        {
            _semaphore.WaitOne();
            bool valid = _users.Exists(
                user => 
                    user.Login.Equals(credentials.Login) && 
                    _passwordHasher.VerifyPassword(user.HashedPassword, credentials.Password)
            );
            _semaphore.Release();
            return valid;
        }
    }
}