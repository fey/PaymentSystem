using System;
using AutoFixture;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;
using Microsoft.EntityFrameworkCore;
using PaymentSystem.Database;
using PaymentSystem.Model.Dto.Auth;
using PaymentSystem.Services.Exceptions;
using PaymentSystem.Services.Interfaces;
using Xunit;

namespace PaymentSystem.Services.Implementations.Tests
{
    public class DbUserRepositoryTests
    {
        private class AutoServicesAttribute : AutoDataAttribute
        {
            public AutoServicesAttribute()
                : base(() =>
                {
                    Fixture fixture = new Fixture();
                    fixture.Customizations.Add(
                        new TypeRelay(
                            typeof(IPassowrdHasher),
                            typeof(BCryptPasswordHasher)
                        )
                    );
                    fixture.Register<UserContext>(() => {
                        var options = new DbContextOptionsBuilder<UserContext>()
                            .UseInMemoryDatabase(databaseName: $"Users{Guid.NewGuid()}")
                            .Options;
                        return new UserContext(options);
                    });
                    return fixture;
                })
            {
            }
        }

        [Theory, AutoServices]
        public async void ShouldAddUser(DbUserRepository repository) =>
            await repository.AddUserAsync(new RegisterModel()
            {
                Username = "логин",
                Password = "пороль",
                PasswordConfirmation = "пороль"
            });

        [Theory, AutoServices]
        public async void ShouldNotAddUserWithDuplicateLogin(DbUserRepository repository)
        {
            RegisterModel registrationInfo = new RegisterModel()
            {
                Username = "логин",
                Password = "пороль",
                PasswordConfirmation = "пороль"
            };
            await repository.AddUserAsync(registrationInfo);
            await Assert.ThrowsAsync<RegistrationException>(() => repository.AddUserAsync(registrationInfo));
        }

        [Theory, AutoServices]
        public async void ShouldNotVerifyNotAddedUser(
            DbUserRepository repository, LoginModel credentials) =>
            await Assert.ThrowsAsync<AuthenticationException>(() => repository.VerifyCredentialsAsync(credentials));

        [Theory, AutoServices]
        public async void ShouldVerifyAddedUser(DbUserRepository repository)
        {
            await repository.AddUserAsync(new RegisterModel()
            {
                Username = "логин",
                Password = "пороль",
                PasswordConfirmation = "пороль"
            });
            await repository.VerifyCredentialsAsync(
                new LoginModel(){
                    Login = "логин",
                    Password = "пороль",
                }
            );
        }
    }
}