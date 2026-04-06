using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReimbursementTrackerApp.DataTransferObjects.Approval;
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
        private readonly IReimbursementService _reimbursementService;

        public ApprovalController(
            IApprovalService service,
            IReimbursementService reimbursementService)
        {
            _service = service;
            _reimbursementService = reimbursementService;
        }

        
        [HttpPost]
        public async Task<IActionResult> Process(ApprovalActionRequestDto request)
        {
            var approverId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _service.ProcessApprovalAsync(approverId, request);
            return Ok("Approval processed successfully");
        }

       
        [HttpGet("{requestId}/history")]
        public async Task<IActionResult> GetHistory(Guid requestId)
        {
            var history = await _service.GetApprovalHistoryAsync(requestId);
            return Ok(history);
        }

        
        [HttpGet("{requestId}/attachment")]
        public async Task<IActionResult> DownloadAttachment(Guid requestId)
        {
            var request = await _reimbursementService.GetByIdAsync(requestId);
            if (request == null || string.IsNullOrEmpty(request.AttachmentPath))
                return NotFound("No attachment found");

            var filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                request.AttachmentPath.TrimStart('/')
            );

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found on server");

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var contentType = GetContentType(filePath);
            return File(fileBytes, contentType, Path.GetFileName(filePath));
        }

        private string GetContentType(string path)
        {
            var types = new Dictionary<string, string>
            {
                { ".pdf", "application/pdf" },
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".png", "image/png" }
            };
            var ext = Path.GetExtension(path).ToLower();
            return types.ContainsKey(ext) ? types[ext] : "application/octet-stream";
        }
    }
}