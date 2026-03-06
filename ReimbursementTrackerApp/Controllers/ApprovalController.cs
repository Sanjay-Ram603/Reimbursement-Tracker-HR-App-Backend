using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReimbursementTrackerApp.DataTransferObjects.Approval;
using ReimbursementTrackerApp.Services.Implementations;
using ReimbursementTrackerApp.Services.Interfaces;
using System.Security.Claims;

namespace ReimbursementTrackerApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ApprovalController : ControllerBase
    {
        private readonly IApprovalService _service;

        public ApprovalController(IApprovalService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Process(ApprovalActionRequestDto request)
        {
            var approverId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _service.ProcessApprovalAsync(approverId, request);
            return Ok();
        }

       
    }
}
