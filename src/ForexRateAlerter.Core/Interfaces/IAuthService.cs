using ForexRateAlerter.Core.DTOs;

namespace ForexRateAlerter.Core.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string Token, string UserId, string Error)> LoginAsync(LoginDto loginDto);
        Task<(bool Success, UserResponseDto? User, string Error)> RegisterAsync(RegisterDto registerDto);
        Task<(bool Success, UserResponseDto? User, string Error)> CreateAdminAsync(RegisterDto registerDto);
        Task<UserResponseDto?> GetUserByIdAsync(int userId);
        string GenerateJwtToken(int userId, string email, string role);
    }
}
