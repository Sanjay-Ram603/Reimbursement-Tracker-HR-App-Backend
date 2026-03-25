using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReimbursementTrackerApp.DataTransferObjects.UserProfile;
using ReimbursementTrackerApp.Services.Interfaces;
using System.Security.Claims;

namespace ReimbursementTrackerApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _service;

        public UserProfileController(IUserProfileService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var profile = await _service.GetProfileAsync(userId);
            return Ok(profile);
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateUserProfileRequestDto request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _service.UpdateProfileAsync(userId, request);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteMyAccount()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _service.DeleteUserAsync(userId);

            return Ok("Account deleted successfully");
        }

    }
}
