using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(id)
            );
        }

        public UserController(IUserRepository repository) =>
            _repository = repository;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginModel credentials)
        {
            await _repository.VerifyCredentialsAsync(credentials);
            Authenticate(credentials.Login);
            return Ok();
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterModel newUser)
        {
            await _repository.AddUserAsync(newUser);
            return  Ok();
        }
    }
}