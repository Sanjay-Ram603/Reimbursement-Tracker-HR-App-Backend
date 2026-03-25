using Microsoft.AspNetCore.Mvc;
using ReimbursementTrackerApp.DataTransferObjects.Authentication;
using ReimbursementTrackerApp.Services.Interfaces;

namespace ReimbursementTrackerApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserRequestDto request)
        {
            var response = await _authenticationService.RegisterAsync(request);
            return Ok(response);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            var response = await _authenticationService.LoginAsync(request);
            return Ok(response);
        }
    }
}
