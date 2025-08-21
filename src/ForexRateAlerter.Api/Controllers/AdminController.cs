using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ForexRateAlerter.Infrastructure.Data;

namespace ForexRateAlerter.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all users (Admin only)
        /// </summary>
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.FirstName,
                    u.LastName,
                    Role = u.Role.ToString(),
                    u.IsActive,
                    u.CreatedAt,
                    AlertCount = u.Alerts.Count(a => a.IsActive)
                })
                .ToListAsync();

            return Ok(new { users });
        }

        /// <summary>
        /// Get all active alerts (Admin only)
        /// </summary>
        [HttpGet("alerts")]
        public async Task<IActionResult> GetAllAlerts()
        {
            var alerts = await _context.Alerts
                .Include(a => a.User)
                .Where(a => a.IsActive)
                .Select(a => new
                {
                    a.Id,
                    a.BaseCurrency,
                    a.TargetCurrency,
                    Condition = a.Condition.ToString(),
                    a.TargetRate,
                    a.CreatedAt,
                    a.LastTriggeredAt,
                    UserEmail = a.User.Email
                })
                .ToListAsync();

            return Ok(new { alerts });
        }

        /// <summary>
        /// Get alert logs (Admin only)
        /// </summary>
        [HttpGet("alert-logs")]
        public async Task<IActionResult> GetAlertLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            var logs = await _context.AlertLogs
                .Include(al => al.Alert)
                .ThenInclude(a => a.User)
                .OrderByDescending(al => al.TriggeredAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(al => new
                {
                    al.Id,
                    AlertId = al.Alert.Id,
                    CurrencyPair = $"{al.Alert.BaseCurrency}/{al.Alert.TargetCurrency}",
                    al.TriggeredRate,
                    al.TargetRate,
                    Condition = al.Condition.ToString(),
                    al.TriggeredAt,
                    al.EmailSent,
                    al.EmailError,
                    UserEmail = al.Alert.User.Email
                })
                .ToListAsync();

            var totalCount = await _context.AlertLogs.CountAsync();

            return Ok(new 
            { 
                logs, 
                pagination = new 
                { 
                    page, 
                    pageSize, 
                    totalCount, 
                    totalPages = (int)Math.Ceiling((double)totalCount / pageSize) 
                } 
            });
        }

        /// <summary>
        /// Get system statistics (Admin only)
        /// </summary>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            var stats = new
            {
                TotalUsers = await _context.Users.CountAsync(u => u.IsActive),
                TotalAlerts = await _context.Alerts.CountAsync(a => a.IsActive),
                AlertsTriggeredToday = await _context.AlertLogs
                    .CountAsync(al => al.TriggeredAt.Date == DateTime.UtcNow.Date),
                AlertsTriggeredThisWeek = await _context.AlertLogs
                    .CountAsync(al => al.TriggeredAt >= DateTime.UtcNow.AddDays(-7)),
                MostPopularCurrencyPairs = await _context.Alerts
                    .Where(a => a.IsActive)
                    .GroupBy(a => new { a.BaseCurrency, a.TargetCurrency })
                    .Select(g => new { CurrencyPair = $"{g.Key.BaseCurrency}/{g.Key.TargetCurrency}", Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(5)
                    .ToListAsync()
            };

            return Ok(stats);
        }
    }
}
