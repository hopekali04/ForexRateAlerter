using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ForexRateAlerter.Core.DTOs;
using ForexRateAlerter.Core.Interfaces;

namespace ForexRateAlerter.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AlertController : ControllerBase
    {
        private readonly IAlertService _alertService;

        public AlertController(IAlertService alertService)
        {
            _alertService = alertService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim!);
        }

        /// <summary>
        /// Create a new alert
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateAlert([FromBody] CreateAlertDto createAlertDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            var alert = await _alertService.CreateAlertAsync(userId, createAlertDto);

            return CreatedAtAction(nameof(GetAlert), new { id = alert.Id }, alert);
        }

        /// <summary>
        /// Get all alerts for the current user
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUserAlerts()
        {
            var userId = GetCurrentUserId();
            var alerts = await _alertService.GetUserAlertsAsync(userId);

            return Ok(new { alerts });
        }

        /// <summary>
        /// Get a specific alert by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAlert(int id)
        {
            var userId = GetCurrentUserId();
            var alert = await _alertService.GetAlertByIdAsync(id, userId);

            if (alert == null)
                return NotFound(new { error = "Alert not found" });

            return Ok(alert);
        }

        /// <summary>
        /// Update an existing alert
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAlert(int id, [FromBody] UpdateAlertDto updateAlertDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            var alert = await _alertService.UpdateAlertAsync(id, userId, updateAlertDto);

            if (alert == null)
                return NotFound(new { error = "Alert not found" });

            return Ok(alert);
        }

        /// <summary>
        /// Delete an alert
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlert(int id)
        {
            var userId = GetCurrentUserId();
            var success = await _alertService.DeleteAlertAsync(id, userId);

            if (!success)
                return NotFound(new { error = "Alert not found" });

            return NoContent();
        }
    }
}
