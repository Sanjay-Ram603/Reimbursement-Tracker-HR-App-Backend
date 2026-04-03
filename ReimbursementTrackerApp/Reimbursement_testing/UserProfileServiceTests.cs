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

        // ─── GET PROFILE ──────────────────────────────────────────────────────────

        [Fact]
        public async Task GetProfileAsync_ExistingUser_ReturnsProfile()
        {
            // Arrange
            var user = MakeUser();
            _userRepoMock.Setup(r => r.GetByIdAsync(user.UserId)).ReturnsAsync(user);

            // Act
            var result = await _service.GetProfileAsync(user.UserId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.FirstName, result.FirstName);
            Assert.Equal(user.LastName, result.LastName);
        }

        [Fact]
        public async Task GetProfileAsync_UserNotFound_ThrowsException()
        {
            // Arrange
            _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.GetProfileAsync(Guid.NewGuid()));
            Assert.Equal("User not found.", ex.Message);
        }

        [Fact]
        public async Task GetProfileAsync_ReturnsCorrectUserId()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = MakeUser(userId);
            _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _service.GetProfileAsync(userId);

            // Assert
            Assert.Equal(userId, result.UserId);
        }

        // ─── UPDATE PROFILE ───────────────────────────────────────────────────────

        [Fact]
        public async Task UpdateProfileAsync_ValidUser_UpdatesFields()
        {
            // Arrange
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

            // Act
            await _service.UpdateProfileAsync(user.UserId, dto);

            // Assert
            Assert.Equal("Jane", user.FirstName);
            Assert.Equal("Smith", user.LastName);
            Assert.Equal("jane@test.com", user.Email);
        }

        [Fact]
        public async Task UpdateProfileAsync_UserNotFound_ThrowsException()
        {
            // Arrange
            _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.UpdateProfileAsync(Guid.NewGuid(), new UpdateUserProfileRequestDto()));
            Assert.Equal("User not found.", ex.Message);
        }

        [Fact]
        public async Task UpdateProfileAsync_NullFields_KeepsOriginalValues()
        {
            // Arrange
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

            // Act
            await _service.UpdateProfileAsync(user.UserId, dto);

            // Assert
            Assert.Equal(originalEmail, user.Email);
            Assert.Equal(originalFirst, user.FirstName);
        }

        [Fact]
        public async Task UpdateProfileAsync_CallsSaveChanges()
        {
            // Arrange
            var user = MakeUser();
            _userRepoMock.Setup(r => r.GetByIdAsync(user.UserId)).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.UpdateAsync(user)).Returns(Task.CompletedTask);
            _userRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _service.UpdateProfileAsync(user.UserId, new UpdateUserProfileRequestDto { FirstName = "Updated" });

            // Assert
            _userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        // ─── DELETE USER ──────────────────────────────────────────────────────────

        [Fact]
        public async Task DeleteUserAsync_ExistingUser_DeletesSuccessfully()
        {
            // Arrange
            var user = MakeUser();
            _userRepoMock.Setup(r => r.GetByIdAsync(user.UserId)).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.DeleteAsync(user)).Returns(Task.CompletedTask);
            _userRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _service.DeleteUserAsync(user.UserId);

            // Assert
            _userRepoMock.Verify(r => r.DeleteAsync(user), Times.Once);
            _userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_UserNotFound_ThrowsException()
        {
            // Arrange
            _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.DeleteUserAsync(Guid.NewGuid()));
            Assert.Equal("User not found", ex.Message);
        }
    }
}
