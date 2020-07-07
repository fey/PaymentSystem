using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentSystem.Model;
using PaymentSystem.Services.Interfaces;

namespace PaymentSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly IPaymentRepository _repository;
        private readonly INotifier<Guid> _notifier;
        private readonly IPaymentValidator _validator;


        public PaymentController(
            ILogger<PaymentController> logger,
            IPaymentRepository repository,
            INotifier<Guid> notifier,
            IPaymentValidator validator
        )
        {
            _logger = logger;
            _repository = repository;
            _notifier = notifier;
            _validator = validator;
        }

        [HttpGet("session")]
        public IActionResult GetPaymentSession(Payment payment)
        {
            return Ok(_repository.RecordPayment(payment));
        }

        [HttpPost("initiate")]
        public IActionResult InitiatePayment(Card cardDetails, Guid sessionId, string callback)
        {
            if (!_repository.SessionIsActive(sessionId))
                return Forbid("Session is not active or payment for this session was already made");
            CardValidationResults validationResult = _validator.ValidateCard(cardDetails);
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
                    if (
                        !String.IsNullOrWhiteSpace(callback) &&
                        Uri.IsWellFormedUriString(callback, UriKind.Absolute)
                    )
                        _notifier.SendAsyncNotification(new Uri(callback), sessionId);
                    return
                        _repository.MakePayment(sessionId, cardDetails, callback) ? 
                        (IActionResult) Ok() : BadRequest(new Error(){
                            Code = 800,
                            Message = "Something went wrong while making payment"
                        });
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
