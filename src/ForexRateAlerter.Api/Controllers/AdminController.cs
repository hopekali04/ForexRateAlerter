using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ForexRateAlerter.Infrastructure.Data;
using System.ComponentModel.DataAnnotations;

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

        /// <summary>
        /// Get a single user by ID (Admin only)
        /// </summary>
        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.FirstName,
                    u.LastName,
                    Role = u.Role.ToString(),
                    u.IsActive,
                    u.CreatedAt,
                    u.UpdatedAt,
                    AlertCount = u.Alerts.Count(a => a.IsActive)
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(user);
        }

        /// <summary>
        /// Create a new user (Admin only)
        /// </summary>
        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                return BadRequest(new { message = "Email already exists" });
            }

            var user = new ForexRateAlerter.Core.Models.User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Role = Enum.Parse<ForexRateAlerter.Core.Models.UserRole>(dto.Role),
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new
            {
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                Role = user.Role.ToString(),
                user.IsActive,
                user.CreatedAt
            });
        }

        /// <summary>
        /// Update a user (Admin only)
        /// </summary>
        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Check if email is being changed and already exists
            if (dto.Email != user.Email && await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                return BadRequest(new { message = "Email already exists" });
            }

            user.Email = dto.Email;
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Role = Enum.Parse<ForexRateAlerter.Core.Models.UserRole>(dto.Role);
            user.IsActive = dto.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            // Update password if provided
            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                Role = user.Role.ToString(),
                user.IsActive,
                user.UpdatedAt
            });
        }

        /// <summary>
        /// Delete a user (Admin only)
        /// </summary>
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Prevent deleting the last admin
            if (user.Role == ForexRateAlerter.Core.Models.UserRole.Admin)
            {
                var adminCount = await _context.Users.CountAsync(u => u.Role == ForexRateAlerter.Core.Models.UserRole.Admin && u.IsActive);
                if (adminCount <= 1)
                {
                    return BadRequest(new { message = "Cannot delete the last active admin user" });
                }
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User deleted successfully" });
        }

        /// <summary>
        /// Toggle user active status (Admin only)
        /// </summary>
        [HttpPatch("users/{id}/toggle-status")]
        public async Task<IActionResult> ToggleUserStatus(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Prevent deactivating the last admin
            if (user.IsActive && user.Role == ForexRateAlerter.Core.Models.UserRole.Admin)
            {
                var activeAdminCount = await _context.Users.CountAsync(u => u.Role == ForexRateAlerter.Core.Models.UserRole.Admin && u.IsActive);
                if (activeAdminCount <= 1)
                {
                    return BadRequest(new { message = "Cannot deactivate the last active admin user" });
                }
            }

            user.IsActive = !user.IsActive;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                user.Id,
                user.IsActive,
                message = $"User {(user.IsActive ? "activated" : "deactivated")} successfully"
            });
        }
    }

    // DTOs for user management
    public class CreateUserDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "User";

        public bool IsActive { get; set; } = true;
    }

    public class UpdateUserDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? Password { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "User";

        public bool IsActive { get; set; } = true;
    }
}
