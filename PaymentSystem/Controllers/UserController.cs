using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PaymentSystem.Model.Dto;
using PaymentSystem.Model.Dto.Auth;
using PaymentSystem.Services.Interfaces;

namespace PaymentSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repository;

        private async void Authenticate(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            ClaimsIdentity id = new ClaimsIdentity(
                claims, "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType
            );
            try
            {
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(id)
                );
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public UserController(IUserRepository repository) =>
            _repository = repository;

        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginModel credentials)
        {
            IActionResult response = null;
            if (_repository.VerifyCredentials(credentials))
            {
                Authenticate(credentials.Login);
                response = Ok();
            }
            else
                response = BadRequest(new Error() 
                    { 
                        Code = 403,
                        Message = "Wrong username or password"
                    });
            return response;
        }

        [HttpGet("logout")]
        public async void Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        [HttpPost("register")]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterModel newUser) =>
            _repository.AddUser(newUser) ? (IActionResult)Ok() : 
            BadRequest(new Error() 
                    { 
                        Code = 1001,
                        Message = "Username is already occupied"
                    });
    }
}