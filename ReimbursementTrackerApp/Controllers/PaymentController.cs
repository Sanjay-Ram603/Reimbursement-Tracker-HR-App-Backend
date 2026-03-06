using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReimbursementTrackerApp.DataTransferObjects.Payment;
using ReimbursementTrackerApp.Services.Interfaces;

namespace ReimbursementTrackerApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _service;

        public PaymentController(IPaymentService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Process(ProcessPaymentRequestDto request)
        {
            await _service.ProcessPaymentAsync(request);
            return Ok();
        }
    }
}
