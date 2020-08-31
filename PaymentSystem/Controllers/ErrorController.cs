using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentSystem.Model.Dto;
using PaymentSystem.Services.Exceptions;

namespace PaymentSystem.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        [AllowAnonymous]
        public IActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context?.Error;
            Error error = null;
            
            switch (exception)
            {
                case AuthenticationException e:
                {
                    return Forbid(e.Message);
                }
                case NotFoundException e:
                {
                    return NotFound(e.Message);
                }
                case ServiceException e:
                {
                    error = new Error() 
                    { 
                        Code = e.Code,
                        Message = e.Message
                    };
                    break;
                }
                default:
                {
                    return StatusCode(500, exception.Message);
                }
            }

            return new BadRequestObjectResult(error);
        }
    }
}