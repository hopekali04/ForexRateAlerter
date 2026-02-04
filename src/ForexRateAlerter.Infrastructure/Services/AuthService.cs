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

using Microsoft.Extensions.Logging;

namespace ForexRateAlerter.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(ApplicationDbContext context, IConfiguration configuration, IEmailService emailService, ILogger<AuthService> logger)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<(bool Success, string Token, UserResponseDto? User, string Error)> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == loginDto.Email.ToLower() && u.IsActive);

                if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                {
                    return (false, string.Empty, null, "Invalid email or password");
                }

                var token = GenerateJwtToken(user.Id, user.Email, user.Role.ToString());
                
                var userResponse = new UserResponseDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role.ToString(),
                    CreatedAt = user.CreatedAt
                };

                return (true, token, userResponse, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for email: {Email}", loginDto.Email);
                return (false, string.Empty, null, "An unexpected error occurred during login. Please try again later.");
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
                _logger.LogError(ex, "Registration failed for email: {Email}", registerDto.Email);
                return (false, null, "An unexpected error occurred during registration. Please try again later.");
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

        public async Task<(bool Success, UserResponseDto? User, string Error)> CreateAdminAsync(RegisterDto registerDto)
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

                // Create new admin user
                var user = new User
                {
                    Email = registerDto.Email.ToLower(),
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                    Role = UserRole.Admin
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
                _logger.LogError(ex, "Admin creation failed for email: {Email}", registerDto.Email);
                return (false, null, "An unexpected error occurred while creating the admin account. Please try again later.");
            }
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