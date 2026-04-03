using Microsoft.Extensions.Configuration;
using Moq;
using ReimbursementTrackerApp.DataTransferObjects.Authentication;
using ReimbursementTrackerApp.Models.Identity;
using ReimbursementTrackerApp.Repositories.Interfaces;
using ReimbursementTrackerApp.Services.Implementations;

namespace Reimbursement_testing
{
    public class AuthenticationServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IRoleRepository> _roleRepoMock;
        private readonly Mock<IConfiguration> _configMock;
        private readonly AuthenticationService _service;

        public AuthenticationServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _roleRepoMock = new Mock<IRoleRepository>();
            _configMock = new Mock<IConfiguration>();

            // Setup JWT config
            _configMock.Setup(c => c["JwtSettings:SecretKey"])
                .Returns("SuperSecretKeyForTestingPurposesOnly1234567890");
            _configMock.Setup(c => c["JwtSettings:Issuer"]).Returns("TestIssuer");
            _configMock.Setup(c => c["JwtSettings:Audience"]).Returns("TestAudience");

            _service = new AuthenticationService(
                _userRepoMock.Object,
                _roleRepoMock.Object,
                _configMock.Object);
        }

        // ─── REGISTER ───────────────────────────────────────────────────────────

        [Fact]
        public async Task RegisterAsync_NewUser_ReturnsRegisterResponse()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var request = new RegisterUserRequestDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com",
                Password = "Pass@123",
                RoleId = roleId
            };

            _userRepoMock.Setup(r => r.GetByEmailAsync(request.Email))
                .ReturnsAsync((User?)null);

            _roleRepoMock.Setup(r => r.GetByRoleNameAsync("Employee"))
                .ReturnsAsync(new Role { RoleId = roleId, RoleName = "Employee", CreatedAt = DateTime.UtcNow });

            _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            _userRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _service.RegisterAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Email, result.Email);
        }

        [Fact]
        public async Task RegisterAsync_ExistingEmail_ThrowsException()
        {
            // Arrange
            var request = new RegisterUserRequestDto
            {
                Email = "existing@test.com",
                Password = "Pass@123",
                FirstName = "Jane",
                LastName = "Doe",
                RoleId = Guid.NewGuid()
            };

            _userRepoMock.Setup(r => r.GetByEmailAsync(request.Email))
                .ReturnsAsync(new User { Email = request.Email, UserId = Guid.NewGuid(), PasswordHash = "", FirstName = "", LastName = "", CreatedAt = DateTime.UtcNow });

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.RegisterAsync(request));
            Assert.Equal("User already exists.", ex.Message);
        }

        [Fact]
        public async Task RegisterAsync_RoleNotFound_ThrowsException()
        {
            // Arrange
            var request = new RegisterUserRequestDto
            {
                Email = "new@test.com",
                Password = "Pass@123",
                FirstName = "New",
                LastName = "User",
                RoleId = Guid.NewGuid()
            };

            _userRepoMock.Setup(r => r.GetByEmailAsync(request.Email))
                .ReturnsAsync((User?)null);

            _roleRepoMock.Setup(r => r.GetByRoleNameAsync("Employee"))
                .ReturnsAsync((Role?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.RegisterAsync(request));
            Assert.Contains("Employee", ex.Message);
        }

        // ─── LOGIN ───────────────────────────────────────────────────────────────

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var password = "Pass@123";
            var hash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = "user@test.com",
                PasswordHash = hash,
                RoleId = roleId,
                FirstName = "Test",
                LastName = "User",
                CreatedAt = DateTime.UtcNow
            };

            _userRepoMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
            _roleRepoMock.Setup(r => r.GetByIdAsync(roleId))
                .ReturnsAsync(new Role { RoleId = roleId, RoleName = "Employee", CreatedAt = DateTime.UtcNow });

            var request = new LoginRequestDto { Email = user.Email, Password = password };

            // Act
            var result = await _service.LoginAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Token);
            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public async Task LoginAsync_UserNotFound_ThrowsException()
        {
            // Arrange
            _userRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);

            var request = new LoginRequestDto { Email = "ghost@test.com", Password = "Pass@123" };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.LoginAsync(request));
            Assert.Equal("Invalid credentials.", ex.Message);
        }

        [Fact]
        public async Task LoginAsync_WrongPassword_ThrowsException()
        {
            // Arrange
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = "user@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPass"),
                RoleId = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "User",
                CreatedAt = DateTime.UtcNow
            };

            _userRepoMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

            var request = new LoginRequestDto { Email = user.Email, Password = "WrongPass" };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.LoginAsync(request));
            Assert.Equal("Invalid credentials.", ex.Message);
        }

        [Fact]
        public async Task LoginAsync_RoleNotFound_ThrowsException()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var password = "Pass@123";
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = "user@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                RoleId = roleId,
                FirstName = "Test",
                LastName = "User",
                CreatedAt = DateTime.UtcNow
            };

            _userRepoMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
            _roleRepoMock.Setup(r => r.GetByIdAsync(roleId)).ReturnsAsync((Role?)null);

            var request = new LoginRequestDto { Email = user.Email, Password = password };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.LoginAsync(request));
            Assert.Equal("User role not found.", ex.Message);
        }
    }
}
