using Moq;
using ReimbursementTrackerApp.DataTransferObjects.UserProfile;
using ReimbursementTrackerApp.Models.Identity;
using ReimbursementTrackerApp.Repositories.Interfaces;
using ReimbursementTrackerApp.Services.Implementations;

namespace Reimbursement_testing
{
    public class UserProfileServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly UserProfileService _service;

        public UserProfileServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _service = new UserProfileService(_userRepoMock.Object);
        }

        private static User MakeUser(Guid? id = null) => new User
        {
            UserId = id ?? Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            PasswordHash = "hash",
            RoleId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };

        [Fact]
        public async Task GetProfileAsync_ExistingUser_ReturnsProfile()
        {
           
            var user = MakeUser();
            _userRepoMock.Setup(r => r.GetByIdAsync(user.UserId)).ReturnsAsync(user);

           
            var result = await _service.GetProfileAsync(user.UserId);

            
            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.FirstName, result.FirstName);
            Assert.Equal(user.LastName, result.LastName);
        }

        [Fact]
        public async Task GetProfileAsync_UserNotFound_ThrowsException()
        {
            
            _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User?)null);

          
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.GetProfileAsync(Guid.NewGuid()));
            Assert.Equal("User not found.", ex.Message);
        }

        [Fact]
        public async Task GetProfileAsync_ReturnsCorrectUserId()
        {
          
            var userId = Guid.NewGuid();
            var user = MakeUser(userId);
            _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            
            var result = await _service.GetProfileAsync(userId);

            
            Assert.Equal(userId, result.UserId);
        }

        [Fact]
        public async Task UpdateProfileAsync_ValidUser_UpdatesFields()
        {
            
            var user = MakeUser();
            _userRepoMock.Setup(r => r.GetByIdAsync(user.UserId)).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.UpdateAsync(user)).Returns(Task.CompletedTask);
            _userRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var dto = new UpdateUserProfileRequestDto
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@test.com"
            };

            await _service.UpdateProfileAsync(user.UserId, dto);

            
            Assert.Equal("Jane", user.FirstName);
            Assert.Equal("Smith", user.LastName);
            Assert.Equal("jane@test.com", user.Email);
        }

        [Fact]
        public async Task UpdateProfileAsync_UserNotFound_ThrowsException()
        {
           
            _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User?)null);

            
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.UpdateProfileAsync(Guid.NewGuid(), new UpdateUserProfileRequestDto()));
            Assert.Equal("User not found.", ex.Message);
        }

        [Fact]
        public async Task UpdateProfileAsync_NullFields_KeepsOriginalValues()
        {
           
            var user = MakeUser();
            var originalEmail = user.Email;
            var originalFirst = user.FirstName;

            _userRepoMock.Setup(r => r.GetByIdAsync(user.UserId)).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.UpdateAsync(user)).Returns(Task.CompletedTask);
            _userRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var dto = new UpdateUserProfileRequestDto
            {
                FirstName = null,
                LastName = null,
                Email = null
            };

            
            await _service.UpdateProfileAsync(user.UserId, dto);

            
            Assert.Equal(originalEmail, user.Email);
            Assert.Equal(originalFirst, user.FirstName);
        }

        [Fact]
        public async Task UpdateProfileAsync_CallsSaveChanges()
        {
            
            var user = MakeUser();
            _userRepoMock.Setup(r => r.GetByIdAsync(user.UserId)).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.UpdateAsync(user)).Returns(Task.CompletedTask);
            _userRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

           
            await _service.UpdateProfileAsync(user.UserId, new UpdateUserProfileRequestDto { FirstName = "Updated" });

            
            _userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_ExistingUser_DeletesSuccessfully()
        {
        
            var user = MakeUser();
            _userRepoMock.Setup(r => r.GetByIdAsync(user.UserId)).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.DeleteAsync(user)).Returns(Task.CompletedTask);
            _userRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            
            await _service.DeleteUserAsync(user.UserId);

            
            _userRepoMock.Verify(r => r.DeleteAsync(user), Times.Once);
            _userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_UserNotFound_ThrowsException()
        {
           
            _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User?)null);

            
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.DeleteUserAsync(Guid.NewGuid()));
            Assert.Equal("User not found", ex.Message);
        }
    }
}
