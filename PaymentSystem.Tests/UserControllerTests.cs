using AutoFixture;
using AutoFixture.AutoMoq;
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
                    return fixture.Customize(new AutoMoqCustomization());
                })
            {
            }
        }

        [Theory, AutoController]
        public void ShouldRegisterNewUser(
            IUserRepository _repository,
            RegisterModel regInfo
        ) => Assert.IsType<OkResult>(
            new UserController(_repository).Register(regInfo)
        );

        [Theory, AutoController]
        public void ShouldNotRegisterSameUser(
            IUserRepository _repository,
            RegisterModel regInfo
        )
        {
            UserController controller = new UserController(_repository);
            controller.Register(regInfo);
            Assert.IsType<BadRequestObjectResult>(controller.Register(regInfo));
        }

        [Theory, AutoController]
        public void ShouldNotLoginInexistentUser(
            IUserRepository _repository, 
            LoginModel credentials
        ) => Assert.IsType<BadRequestObjectResult>(
            new UserController(_repository).Login(credentials)
        );

        [Theory, AutoController]
        public void ShouldNotLoginUserByWrongPassword(
            IUserRepository _repository, 
            RegisterModel regInfo
        )
        {
            UserController controller = new UserController(_repository);
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
            IUserRepository _repository, 
            RegisterModel regInfo
        )
        {
            UserController controller = new UserController(_repository);
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