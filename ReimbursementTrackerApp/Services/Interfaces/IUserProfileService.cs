using ReimbursementTrackerApp.DataTransferObjects.UserProfile;

namespace ReimbursementTrackerApp.Services.Interfaces
{
    public interface IUserProfileService
    {
        Task<UserProfileResponseDto> GetProfileAsync(Guid userId);
        Task UpdateProfileAsync(Guid userId, UpdateUserProfileRequestDto request);

        Task DeleteUserAsync(Guid userId);

    }
}
