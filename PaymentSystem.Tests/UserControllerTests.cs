using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using PaymentSystem.Database;
using PaymentSystem.Model.Dto.Auth;
using PaymentSystem.Services.Exceptions;
using PaymentSystem.Services.Implementations;
using PaymentSystem.Services.Interfaces;
using Xunit;

namespace PaymentSystem.Controllers.Tests
{
    public class UserControllerTests
    {
        private class AutoControllerAttribute: AutoDataAttribute
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
                            typeof(DbUserRepository)
                        )
                    );
                    fixture.Register<UserContext>(() => {
                        var options = new DbContextOptionsBuilder<UserContext>()
                            .UseInMemoryDatabase(databaseName: $"2Users{Guid.NewGuid()}")
                            .Options;
                        return new UserContext(options);
                    });
                    return fixture;
                })
            {
            }
        }

        [Theory, AutoController]
        public async void ShouldRegisterNewUser(
            [NoAutoProperties]UserController controller,
            RegisterModel regInfo
        ) => Assert.IsType<OkResult>(await controller.Register(regInfo));

        [Theory, AutoController]
        public async void ShouldNotRegisterSameUser(
            [NoAutoProperties]UserController controller,
            RegisterModel regInfo
        )
        {
            await controller.Register(regInfo);
            await Assert.ThrowsAsync<RegistrationException>(() => controller.Register(regInfo));
        }

        [Theory, AutoController]
        public async void ShouldNotLoginInexistentUser(
            [NoAutoProperties]UserController controller,
            LoginModel credentials
        ) => await Assert.ThrowsAsync<AuthenticationException>(() => controller.Login(credentials));

        [Theory, AutoController]
        public async void ShouldNotLoginUserByWrongPassword(
            [NoAutoProperties]UserController controller,
            RegisterModel regInfo
        )
        {
            await controller.Register(regInfo);
            await Assert.ThrowsAsync<AuthenticationException>(
                () => controller.Login(new LoginModel()
                {
                    Login = regInfo.Username,
                    Password = regInfo.Password.GetHashCode().ToString()
                }));
        }

        [Theory, AutoController]
        public async void ShouldLoginExistingUser(
            [NoAutoProperties]UserController controller,
            RegisterModel regInfo
        )
        {
            var authenticationServiceMock = new Mock<IAuthenticationService>();
            authenticationServiceMock
                .Setup(a => a.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(s => s.GetService(typeof(IAuthenticationService)))
                .Returns(authenticationServiceMock.Object);

            controller.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                    {
                        RequestServices = serviceProviderMock.Object
                    }
                };

            await controller.Register(regInfo);
            Assert.IsType<OkResult>(
                await controller.Login(new LoginModel()
                {
                    Login = regInfo.Username,
                    Password = regInfo.Password
                }));
        }
    }
}