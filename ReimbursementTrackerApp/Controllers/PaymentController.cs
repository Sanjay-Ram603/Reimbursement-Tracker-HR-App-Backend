using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReimbursementTrackerApp.DataTransferObjects.Payment;
using ReimbursementTrackerApp.Services.Implementations;
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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllPaymentsAsync();
            return Ok(result);
        }

        [HttpGet("request/{requestId}")]
        public async Task<IActionResult> GetByRequest(Guid requestId)
        {
            var result = await _service.GetByRequestIdAsync(requestId);
            return Ok(result);
        }

    }
}
