using System;
using AutoFixture;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;
using Microsoft.EntityFrameworkCore;
using PaymentSystem.Database;
using PaymentSystem.Model.Dto.Auth;
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
        public void ShouldAddUser(DbUserRepository repository) =>
            Assert.True(repository.AddUser(new RegisterModel()
            {
                Username = "логин",
                Password = "пороль",
                PasswordConfirmation = "пороль"
            }));

        [Theory, AutoServices]
        public void ShouldNotAddUserWithDuplicateLogin(DbUserRepository repository)
        {
            RegisterModel registrationInfo = new RegisterModel()
            {
                Username = "логин",
                Password = "пороль",
                PasswordConfirmation = "пороль"
            };
            repository.AddUser(registrationInfo);
            Assert.False(repository.AddUser(registrationInfo));
        }

        [Theory, AutoServices]
        public void ShouldNotVerifyNotAddedUser(
            DbUserRepository repository, LoginModel credentials) =>
            Assert.False(repository.VerifyCredentials(credentials));

        [Theory, AutoServices]
        public void ShouldVerifyAddedUser(DbUserRepository repository)
        {
            repository.AddUser(new RegisterModel()
            {
                Username = "логин",
                Password = "пороль",
                PasswordConfirmation = "пороль"
            });
            Assert.True(repository.VerifyCredentials(
                new LoginModel(){
                    Login = "логин",
                    Password = "пороль",
                }
            ));
        }
    }
}