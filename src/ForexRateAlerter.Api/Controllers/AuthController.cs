using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ForexRateAlerter.Core.DTOs;
using ForexRateAlerter.Core.Interfaces;

namespace ForexRateAlerter.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(registerDto);

            if (!result.Success)
                return BadRequest(new { error = result.Error });

            return Ok(new { message = "User registered successfully", user = result.User });
        }

        /// <summary>
        /// Login user
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(loginDto);

            if (!result.Success)
                return Unauthorized(new { error = result.Error });

            return Ok(new 
            { 
                token = result.Token, 
                user = result.User,
                message = "Login successful" 
            });
        }

        /// <summary>
        /// Create a new admin user (Admin access required)
        /// </summary>
        [HttpPost("create-admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAdmin([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.CreateAdminAsync(registerDto);

            if (!result.Success)
                return BadRequest(new { error = result.Error });

            return Ok(new { message = "Admin user created successfully", user = result.User });
        }
    }
}
