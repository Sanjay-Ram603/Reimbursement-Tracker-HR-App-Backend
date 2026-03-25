using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReimbursementTrackerApp.Models.Identity;
using ReimbursementTrackerApp.Services.Interfaces;

namespace ReimbursementTrackerApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _service;

        public ReportController(IReportService service)
        {
            _service = service;
        }

        [HttpGet]
        //[Authorize(Roles="Admin")]
        public async Task<IActionResult> Generate(
    DateTime fromDate,
    DateTime toDate,
    string? status, 
    int startIndex = 0,
    int pageSize = 5)
        {
            var report = await _service.GenerateReportAsync(fromDate, toDate, status, startIndex, pageSize);
            return Ok(report);
        }

    }
}
