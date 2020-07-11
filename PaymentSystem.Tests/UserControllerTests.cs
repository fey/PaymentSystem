using AutoFixture;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Mvc;
using PaymentSystem.Model.Dto.Auth;
using PaymentSystem.Services.Implementations;
using PaymentSystem.Services.Interfaces;
using Xunit;

namespace PaymentSystem.Controllers.Tests
{
    public class UserControllerTests
    {
        class AutoControllerAttribute: AutoDataAttribute
        {
            public AutoControllerAttribute()
                : base(() =>
                {
                    Fixture fixture = new Fixture();
                    fixture.Customizations.Add(
                        new TypeRelay(
                            typeof(IPassowrdHasher),
                            typeof(BCryptPasswordHasher)
                        )
                    );
                    fixture.Customizations.Add(
                        new TypeRelay(
                            typeof(IUserRepository), 
                            typeof(StubUserRepository)
                        )
                    );
                    return fixture;
                })
            {
            }
        }

        [Theory, AutoController]
        public void ShouldRegisterNewUser(
            [NoAutoProperties]UserController controller,
            RegisterModel regInfo
        ) => Assert.IsType<OkResult>(controller.Register(regInfo));

        [Theory, AutoController]
        public void ShouldNotRegisterSameUser(
            [NoAutoProperties]UserController controller,
            RegisterModel regInfo
        )
        {
            controller.Register(regInfo);
            Assert.IsType<BadRequestObjectResult>(controller.Register(regInfo));
        }

        [Theory, AutoController]
        public void ShouldNotLoginInexistentUser(
            [NoAutoProperties]UserController controller,
            LoginModel credentials
        ) => Assert.IsType<BadRequestObjectResult>(controller.Login(credentials));

        [Theory, AutoController]
        public void ShouldNotLoginUserByWrongPassword(
            [NoAutoProperties]UserController controller,
            RegisterModel regInfo
        )
        {
            controller.Register(regInfo);
            Assert.IsType<BadRequestObjectResult>(
                controller.Login(new LoginModel()
                {
                    Login = regInfo.Username,
                    Password = regInfo.Password.GetHashCode().ToString()
                }));
        }

        [Theory, AutoController]
        public void ShouldLoginExistingUser(
            [NoAutoProperties]UserController controller,
            RegisterModel regInfo
        )
        {
            controller.Register(regInfo);
            Assert.IsType<OkResult>(
                controller.Login(new LoginModel()
                {
                    Login = regInfo.Username,
                    Password = regInfo.Password
                }));
        }
    }
}