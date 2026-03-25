
using ReimbursementTrackerApp.DataTransferObjects.UserProfile;
using ReimbursementTrackerApp.Repositories.Interfaces;
using ReimbursementTrackerApp.Services.Interfaces;

namespace ReimbursementTrackerApp.Services.Implementations
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserRepository _userRepository;

        public UserProfileService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserProfileResponseDto> GetProfileAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found.");

            return new UserProfileResponseDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        public async Task UpdateProfileAsync(Guid userId, UpdateUserProfileRequestDto request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found.");

            user.Email = request.Email ?? user.Email;
            user.FirstName = request.FirstName ?? user.FirstName;
            user.LastName = request.LastName ?? user.LastName;
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new Exception("User not found");

            await _userRepository.DeleteAsync(user);
            await _userRepository.SaveChangesAsync();
        }

    }
}
