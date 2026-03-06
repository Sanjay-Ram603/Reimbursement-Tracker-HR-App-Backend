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
        private readonly IConfiguration _configuration;

        public AuthenticationService(
            IUserRepository userRepository,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterUserRequestDto request)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
                throw new Exception("User already exists.");

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

            var token = GenerateJwtToken(user);

            return new RegisterResponseDto
            {
                UserId = user.UserId,
                Email = user.Email
            };
        }

        public async Task<AuthenticationResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
                throw new Exception("Invalid credentials.");

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
                throw new Exception("Invalid credentials.");

            var token = GenerateJwtToken(user);

            return new AuthenticationResponseDto
            {
                UserId = user.UserId,
                Email = user.Email,
                Token = token
            };
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
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
