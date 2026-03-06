using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReimbursementTrackerApp.DataTransferObjects.Reimbursement;
using ReimbursementTrackerApp.Services.Implementations;
using ReimbursementTrackerApp.Services.Interfaces;
using System.Security.Claims;

namespace ReimbursementTrackerApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReimbursementRequestController : ControllerBase
    {
        private readonly IReimbursementService _service;

        public ReimbursementRequestController(IReimbursementService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateReimbursementRequestDto request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var id = await _service.CreateRequestAsync(userId, request);
            return Ok(id);
        }

        [HttpGet]
        public async Task<IActionResult> GetMyRequests()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _service.GetUserRequestsAsync(userId);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStatus(Guid id, UpdateReimbursementStatusRequestDto request)
        {
            await _service.UpdateStatusAsync(id, request);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteReimbursementRequest(id);

            if (!result)
                return NotFound("Request not found");

            return Ok("Request deleted successfully");
        }

    }
}
