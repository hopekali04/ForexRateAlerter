using ForexRateAlerter.Core.DTOs;
using ForexRateAlerter.Core.Models;

namespace ForexRateAlerter.Core.Interfaces
{
    public interface IAlertService
    {
        Task<AlertResponseDto> CreateAlertAsync(int userId, CreateAlertDto createAlertDto);
        Task<AlertResponseDto?> GetAlertByIdAsync(int alertId, int userId);
        Task<IEnumerable<AlertResponseDto>> GetUserAlertsAsync(int userId);
        Task<AlertResponseDto?> UpdateAlertAsync(int alertId, int userId, UpdateAlertDto updateAlertDto);
        Task<bool> DeleteAlertAsync(int alertId, int userId);
        Task<int> ProcessAlertsAsync();
    }
}
