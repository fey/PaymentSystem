using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentSystem.Model;
using PaymentSystem.Model.Dto;
using PaymentSystem.Model.Dto.Payments;
using PaymentSystem.Services.Interfaces;

namespace PaymentSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentRepository _repository;
        private readonly INotifier _notifier;
        private readonly ICardValidator _validator;


        public PaymentController(
            IPaymentRepository repository,
            INotifier notifier,
            ICardValidator validator
        )
        {
            _repository = repository;
            _notifier = notifier;
            _validator = validator;
        }

        [HttpGet("history")]
        [Authorize]
        public IActionResult GetPaymentHistory(
            [FromQuery]DateTime periodStart, [FromQuery]DateTime periodEnd
        ) 
        {
            try
            {
                return Ok(_repository.GetPaymentHistoryAsync(periodStart, periodEnd));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("session")]
        public async Task<IActionResult> GetPaymentSession([FromBody]PaymentRequest payment)
        {
            if (payment.Sum > 0)
                try
                {
                    return Ok(await _repository.RecordPaymentAsync(payment));
                }
                catch
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            else
                return BadRequest();
        }
        
        [HttpPost("initiate")]
        public async Task<IActionResult> InitiatePayment(
            [FromBody]Card cardDetails, [FromQuery]Guid sessionId, [FromQuery]string callback
        )
        {
            CardValidationResults validationResult;
            try
            {
                if (!(await _repository.SessionIsActiveAsync(sessionId)))
                    return NotFound(
                        new Error() 
                        { 
                            Code = 404,
                            Message = "Session is not active or payment for this session was already made"
                        });
                validationResult = _validator.ValidateCard(cardDetails);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            switch (validationResult)
            {
                case CardValidationResults.Expired:
                    return BadRequest(new Error() 
                    { 
                        Code = (int)validationResult,
                        Message = "Card is expired"
                    });
                case CardValidationResults.InvalidNumber:
                    return BadRequest(new Error() 
                    { 
                        Code = (int)validationResult,
                        Message = "Invalid card number"
                    });
                case CardValidationResults.InvalidSecurityCode:
                    return BadRequest(new Error() 
                    { 
                        Code = (int)validationResult, 
                        Message = "Invalid card security code"
                    });
                case CardValidationResults.Valid:
                {
                    try
                    {
                        if (
                            !String.IsNullOrWhiteSpace(callback) &&
                            Uri.IsWellFormedUriString(callback, UriKind.Absolute)
                        )
                            await _notifier.SendAsyncNotification(new Uri(callback), sessionId.ToString());
                        return
                            await _repository.MakePaymentAsync(sessionId, cardDetails) ? 
                            (IActionResult) Ok() : BadRequest(new Error(){
                                Code = 800,
                                Message = "Something went wrong while making payment"
                            });
                    }
                    catch
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    }
                }
                default:
                    return BadRequest(new Error() 
                    { 
                        Code = (int)validationResult,
                        Message = "Uh-oh, something is wrong with your card :("
                    });
            }
        }
    }
}
