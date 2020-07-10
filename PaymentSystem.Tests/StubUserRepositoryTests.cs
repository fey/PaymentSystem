using AutoFixture;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;
using PaymentSystem.Model.Dto.Auth;
using PaymentSystem.Services.Interfaces;
using Xunit;

namespace PaymentSystem.Services.Implementations.Tests
{
    public class StubUserRepositoryTests
    {
        class AutoDataWithHasherAttribute : AutoDataAttribute
        {
            public AutoDataWithHasherAttribute()
                : base(() =>
                {
                    Fixture fixture = new Fixture();
                    fixture.Customizations.Add(
                        new TypeRelay(typeof(IPassowrdHasher), typeof(BCryptPasswordHasher))
                    );
                    return fixture;
                })
            {
            }
        }

        [Theory, AutoDataWithHasher]
        public void ShouldAddUser(StubUserRepository repository) =>
            Assert.True(repository.AddUser(new RegisterModel()
            {
                Username = "логин",
                Password = "пороль",
                PasswordConfirmation = "пороль"
            }));

        [Theory, AutoDataWithHasher]
        public void ShouldNotAddUserWithDuplicateLogin(StubUserRepository repository)
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

        [Theory, AutoDataWithHasher]
        public void ShouldNotVerifyNotAddedUser(
            StubUserRepository repository, LoginModel credentials) =>
            Assert.False(repository.VerifyCredentials(credentials));

        [Theory, AutoDataWithHasher]
        public void ShouldVerifyAddedUser(StubUserRepository repository)
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