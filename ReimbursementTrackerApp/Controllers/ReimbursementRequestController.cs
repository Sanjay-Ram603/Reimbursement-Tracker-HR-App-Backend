using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReimbursementTrackerApp.DataTransferObjects.Reimbursement;
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

        // ✅ CREATE REQUEST (WITH FILE UPLOAD)
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateReimbursementRequestDto request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var id = await _service.CreateRequestAsync(userId, request);

            return Ok(id);
        }

        // ✅ GET USER REQUESTS
        [HttpGet]
        public async Task<IActionResult> GetMyRequests()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _service.GetUserRequestsAsync(userId);

            return Ok(result);
        }

        // ✅ GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);

            if (result == null)
                return NotFound("Request not found");

            return Ok(result);
        }

        // ✅ UPDATE (EDIT ONLY)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateReimbursementStatusRequestDto request)
        {
            await _service.UpdateRequestAsync(id, request);
            return Ok("Request updated successfully");
        }

        // ✅ DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteReimbursementRequest(id);

            if (!result)
                return NotFound("Request not found");

            return Ok("Request deleted successfully");
        }

        // ✅ GET ALL REQUESTS (Admin + Finance)
        [HttpGet("all")]
        public async Task<IActionResult> GetAllRequests()
        {
            var result = await _service.GetAllRequestsAsync();
            return Ok(result);
        }

        // ✅ DOWNLOAD ATTACHMENT
        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadAttachment(Guid id)
        {
            var request = await _service.GetByIdAsync(id);

            if (request == null || string.IsNullOrEmpty(request.AttachmentPath))
                return NotFound("File not found");

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

        // 🔧 HELPER METHOD
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
