using Microsoft.EntityFrameworkCore;
using ForexRateAlerter.Core.DTOs;
using ForexRateAlerter.Core.Interfaces;
using ForexRateAlerter.Core.Models;
using ForexRateAlerter.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace ForexRateAlerter.Infrastructure.Services
{
    public class AlertService : IAlertService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExchangeRateService _exchangeRateService;
        private readonly IEmailService _emailService;
        private readonly ILogger<AlertService> _logger;

        public AlertService(ApplicationDbContext context, IExchangeRateService exchangeRateService, 
            IEmailService emailService, ILogger<AlertService> logger)
        {
            _context = context;
            _exchangeRateService = exchangeRateService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<AlertResponseDto> CreateAlertAsync(int userId, CreateAlertDto createAlertDto)
        {
            var alert = new Alert
            {
                UserId = userId,
                BaseCurrency = createAlertDto.BaseCurrency.ToUpper(),
                TargetCurrency = createAlertDto.TargetCurrency.ToUpper(),
                Condition = createAlertDto.Condition,
                TargetRate = createAlertDto.TargetRate
            };

            _context.Alerts.Add(alert);
            await _context.SaveChangesAsync();

            return new AlertResponseDto
            {
                Id = alert.Id,
                BaseCurrency = alert.BaseCurrency,
                TargetCurrency = alert.TargetCurrency,
                Condition = alert.Condition.ToString(),
                TargetRate = alert.TargetRate,
                IsActive = alert.IsActive,
                CreatedAt = alert.CreatedAt,
                LastTriggeredAt = alert.LastTriggeredAt
            };
        }

        public async Task<AlertResponseDto?> GetAlertByIdAsync(int alertId, int userId)
        {
            var alert = await _context.Alerts
                .FirstOrDefaultAsync(a => a.Id == alertId && a.UserId == userId);

            if (alert == null) return null;

            return new AlertResponseDto
            {
                Id = alert.Id,
                BaseCurrency = alert.BaseCurrency,
                TargetCurrency = alert.TargetCurrency,
                Condition = alert.Condition.ToString(),
                TargetRate = alert.TargetRate,
                IsActive = alert.IsActive,
                CreatedAt = alert.CreatedAt,
                LastTriggeredAt = alert.LastTriggeredAt
            };
        }

        public async Task<IEnumerable<AlertResponseDto>> GetUserAlertsAsync(int userId)
        {
            var alerts = await _context.Alerts
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return alerts.Select(alert => new AlertResponseDto
            {
                Id = alert.Id,
                BaseCurrency = alert.BaseCurrency,
                TargetCurrency = alert.TargetCurrency,
                Condition = alert.Condition.ToString(),
                TargetRate = alert.TargetRate,
                IsActive = alert.IsActive,
                CreatedAt = alert.CreatedAt,
                LastTriggeredAt = alert.LastTriggeredAt
            });
        }

        public async Task<AlertResponseDto?> UpdateAlertAsync(int alertId, int userId, UpdateAlertDto updateAlertDto)
        {
            var alert = await _context.Alerts
                .FirstOrDefaultAsync(a => a.Id == alertId && a.UserId == userId);

            if (alert == null) return null;

            if (updateAlertDto.Condition.HasValue)
                alert.Condition = updateAlertDto.Condition.Value;

            if (updateAlertDto.TargetRate.HasValue)
                alert.TargetRate = updateAlertDto.TargetRate.Value;

            if (updateAlertDto.IsActive.HasValue)
                alert.IsActive = updateAlertDto.IsActive.Value;

            alert.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new AlertResponseDto
            {
                Id = alert.Id,
                BaseCurrency = alert.BaseCurrency,
                TargetCurrency = alert.TargetCurrency,
                Condition = alert.Condition.ToString(),
                TargetRate = alert.TargetRate,
                IsActive = alert.IsActive,
                CreatedAt = alert.CreatedAt,
                LastTriggeredAt = alert.LastTriggeredAt
            };
        }

        public async Task<bool> DeleteAlertAsync(int alertId, int userId)
        {
            var alert = await _context.Alerts
                .FirstOrDefaultAsync(a => a.Id == alertId && a.UserId == userId);

            if (alert == null) return false;

            _context.Alerts.Remove(alert);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<int> ProcessAlertsAsync()
        {
            var activeAlerts = await _context.Alerts
                .Include(a => a.User)
                .Where(a => a.IsActive)
                .ToListAsync();

            var processedCount = 0;

            foreach (var alert in activeAlerts)
            {
                try
                {
                    var currentRate = await _exchangeRateService.GetLatestRateAsync(
                        alert.BaseCurrency, alert.TargetCurrency);

                    if (currentRate == null) continue;

                    var isTriggered = alert.Condition switch
                    {
                        AlertCondition.GreaterThan => currentRate.Rate > alert.TargetRate,
                        AlertCondition.LessThan => currentRate.Rate < alert.TargetRate,
                        AlertCondition.EqualTo => Math.Abs(currentRate.Rate - alert.TargetRate) < 0.001m,
                        _ => false
                    };

                    if (isTriggered)
                    {
                        // Create alert log
                        var alertLog = new AlertLog
                        {
                            AlertId = alert.Id,
                            TriggeredRate = currentRate.Rate,
                            TargetRate = alert.TargetRate,
                            Condition = alert.Condition,
                            TriggeredAt = DateTime.UtcNow
                        };

                        // Send email notification
                        var emailSent = await _emailService.SendAlertEmailAsync(
                            alert.User.Email,
                            alert.BaseCurrency,
                            alert.TargetCurrency,
                            currentRate.Rate,
                            alert.TargetRate,
                            alert.Condition.ToString()
                        );

                        alertLog.EmailSent = emailSent;
                        if (!emailSent)
                        {
                            alertLog.EmailError = "Failed to send email notification";
                        }

                        _context.AlertLogs.Add(alertLog);

                        // Update alert last triggered time
                        alert.LastTriggeredAt = DateTime.UtcNow;

                        processedCount++;
                        _logger.LogInformation($"Alert {alert.Id} triggered for user {alert.User.Email}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing alert {alert.Id}");
                }
            }

            if (processedCount > 0)
            {
                await _context.SaveChangesAsync();
            }

            return processedCount;
        }
    }
}