using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> Login([FromBody]LoginModel credentials)
        {
            IActionResult response = null;
            try
                {
                if (await _repository.VerifyCredentialsAsync(credentials))
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
            }
            catch
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return response;
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterModel newUser)
        {
            try
            {
                return await _repository.AddUserAsync(newUser) ? (IActionResult)Ok() : 
                BadRequest(new Error() 
                        { 
                            Code = 1001,
                            Message = "Username is already occupied"
                        });
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}