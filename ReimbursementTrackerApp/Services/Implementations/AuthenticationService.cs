using Microsoft.IdentityModel.Tokens;
using ReimbursementTrackerApp.DataTransferObjects.Authentication;
using ReimbursementTrackerApp.Models.Identity;
using ReimbursementTrackerApp.Repositories.Interfaces;
using ReimbursementTrackerApp.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ReimbursementTrackerApp.Services.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IConfiguration _configuration;

        public AuthenticationService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _configuration = configuration;
        }

        // 🔥 REGISTER 
        public async Task<RegisterResponseDto> RegisterAsync(RegisterUserRequestDto request)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
                throw new Exception("User already exists.");

            // ✅ Always assign Employee role
            var role = await _roleRepository.GetByRoleNameAsync("Employee");

            if (role == null)
                throw new Exception("Default role 'Employee' not found in database.");

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow,
                RoleId = request.RoleId
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return new RegisterResponseDto
            {
                UserId = user.UserId,
                Email = user.Email
            };
        }

        // 🔥 LOGIN
        public async Task<AuthenticationResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
                throw new Exception("Invalid credentials.");

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
                throw new Exception("Invalid credentials.");

            // ✅ Generate token with role
            var token = await GenerateJwtToken(user);

            return new AuthenticationResponseDto
            {
                UserId = user.UserId,
                Email = user.Email,
                Token = token
            };
        }

        // 🔥 JWT TOKEN WITH ROLE
        private async Task<string> GenerateJwtToken(User user)
        {
            var role = await _roleRepository.GetByIdAsync(user.RoleId);

            if (role == null)
                throw new Exception("User role not found.");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),

                // 🔥 MOST IMPORTANT (Role-based auth)
                new Claim(ClaimTypes.Role, role.RoleName)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
