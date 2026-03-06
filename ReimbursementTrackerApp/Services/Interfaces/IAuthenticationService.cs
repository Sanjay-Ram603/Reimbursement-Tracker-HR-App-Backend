using ReimbursementTrackerApp.DataTransferObjects.Authentication;

namespace ReimbursementTrackerApp.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<RegisterResponseDto> RegisterAsync(RegisterUserRequestDto request);
        Task<AuthenticationResponseDto> LoginAsync(LoginRequestDto request);
    }
}
