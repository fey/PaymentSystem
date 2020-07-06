using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PaymentSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(ILogger<PaymentController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public String InitiateSession(decimal paymentSum, string destination)
        {
            Guid sessionId = Guid.NewGuid();
            return sessionId.ToString();
        }

        [HttpGet]
        public void InitiatePayment()
        {

        }
    }
}
