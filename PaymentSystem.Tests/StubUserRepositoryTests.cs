using PaymentSystem.Model.Dto.Auth;
using Xunit;

namespace PaymentSystem.Services.Implementations.Tests
{
    public class StubUserRepositoryTests
    {
        [Fact]
        public void ShouldAddUser()
        {
            StubUserRepository repository = new StubUserRepository(new BCryptPasswordHasher());
            Assert.True(repository.AddUser(new RegisterModel()
            {
                Username = "логин",
                Password = "пороль",
                PasswordConfirmation = "пороль"
            }));
        }

        [Fact]
        public void ShouldNotAddUserWithDuplicateLogin()
        {
            StubUserRepository repository = new StubUserRepository(new BCryptPasswordHasher());
            RegisterModel registrationInfo = new RegisterModel()
            {
                Username = "логин",
                Password = "пороль",
                PasswordConfirmation = "пороль"
            };
            repository.AddUser(registrationInfo);
            Assert.False(repository.AddUser(registrationInfo));
        }

        [Fact]
        public void ShouldNotVerifyNotAddedUser()
        {
            StubUserRepository repository = new StubUserRepository(new BCryptPasswordHasher());
            Assert.False(repository.VerifyCredentials(
                new LoginModel(){
                    Login = "логин",
                    Password = "пороль",
                }
            ));
        }

        [Fact]
        public void ShouldVerifyAddedUser()
        {
            StubUserRepository repository = new StubUserRepository(new BCryptPasswordHasher());
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