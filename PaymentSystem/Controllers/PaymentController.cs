using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentSystem.Model;
using PaymentSystem.Model.Common;
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
        private readonly INotifier<Guid> _notifier;
        private readonly ICardValidator _validator;


        public PaymentController(
            IPaymentRepository repository,
            INotifier<Guid> notifier,
            ICardValidator validator
        )
        {
            _repository = repository;
            _notifier = notifier;
            _validator = validator;
        }

        [HttpGet("history")]
        [Authorize]
        public ActionResult<List<PaymentRecord>> GetPaymentHistory(DateTime periodStart, DateTime periodEnd)
        {
            return _repository.GetPaymentHistory(periodStart, periodEnd);
        }

        [HttpGet("session")]
        public IActionResult GetPaymentSession(Payment payment)
        {
            return Ok(_repository.RecordPayment(payment));
        }

        [HttpPost("initiate")]
        public async Task<IActionResult> InitiatePayment(Card cardDetails, Guid sessionId, string callback)
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
                        await _notifier.SendAsyncNotification(new Uri(callback), sessionId);
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
