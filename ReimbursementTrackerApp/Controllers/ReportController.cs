using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Generate(
            DateTime fromDate,
            DateTime toDate,
            string? status,
            string? role,
            int startIndex = 0,
            int pageSize = 5)
        {
            var report = await _service.GenerateReportAsync(
                fromDate, toDate, status, role, startIndex, pageSize);
            return Ok(report);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetCount(
            DateTime fromDate,
            DateTime toDate,
            string? status,
            string? role)
        {
            var count = await _service.GetTotalCountAsync(
                fromDate, toDate, status, role);
            return Ok(count);
        }
    }
}