using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using ForexRateAlerter.Core.DTOs;
using ForexRateAlerter.Core.Interfaces;
using ForexRateAlerter.Core.Models;
using ForexRateAlerter.Infrastructure.Data;
using Microsoft.Extensions.Configuration;

namespace ForexRateAlerter.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthService(ApplicationDbContext context, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<(bool Success, string Token, string Error)> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == loginDto.Email.ToLower() && u.IsActive);

                if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                {
                    return (false, string.Empty, "Invalid email or password");
                }

                var token = GenerateJwtToken(user.Id, user.Email, user.Role.ToString());
                return (true, token, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, string.Empty, $"Login failed: {ex.Message}");
            }
        }

        public async Task<(bool Success, UserResponseDto? User, string Error)> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == registerDto.Email.ToLower());

                if (existingUser != null)
                {
                    return (false, null, "User with this email already exists");
                }

                // Create new user
                var user = new User
                {
                    Email = registerDto.Email.ToLower(),
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                    Role = UserRole.User
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Send welcome email (fire and forget)
                _ = Task.Run(() => _emailService.SendWelcomeEmailAsync(user.Email, user.FirstName));

                var userResponse = new UserResponseDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role.ToString(),
                    CreatedAt = user.CreatedAt
                };

                return (true, userResponse, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, null, $"Registration failed: {ex.Message}");
            }
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);

            if (user == null) return null;

            return new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString(),
                CreatedAt = user.CreatedAt
            };
        }

        public string GenerateJwtToken(int userId, string email, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}