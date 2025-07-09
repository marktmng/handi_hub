using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using System.Security.Claims;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly AuthHelper _authHelper;
        private readonly IConfiguration _config;

        public AuthController(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _authHelper = new AuthHelper(config);
        }

        // -------------------- Register --------------------
        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegistrationDto userForRegistration)
        {
            if (userForRegistration.Password != userForRegistration.PasswordConfirm)
                return BadRequest("Passwords do not match");

            // ✅ Safe email existence check via Users table
            var exists = await _context.Users.AnyAsync(u => u.Email == userForRegistration.Email);
            if (exists)
                return BadRequest("User already exists with this email");

            // Hash and store password using helper
            var loginDto = new LoginDto
            {
                Email = userForRegistration.Email,
                Password = userForRegistration.Password
            };

            if (!_authHelper.SetPassword(loginDto))
                return StatusCode(500, "Failed to hash password");

            // Save user to Users table
            var user = new User
            {
                FirstName = userForRegistration.FirstName,
                LastName = userForRegistration.LastName,
                UserName = userForRegistration.UserName,
                Email = userForRegistration.Email,
                PhoneNumber = userForRegistration.PhoneNumber,
                Role = userForRegistration.Role,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Registration successful");
        }

        // -------------------- Login --------------------
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto userForLogin)
        {
            var results = await _context.LoginConfirmationDtos
                .FromSqlRaw("EXEC HandiHub.spLoginConfirmation_Get @Email = {0}", userForLogin.Email)
                .ToListAsync();

            var result = results.FirstOrDefault();

            if (result == null)
                return Unauthorized("User not found");

            byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, result.PasswordSalt);
            if (!passwordHash.SequenceEqual(result.PasswordHash))
                return Unauthorized("Invalid password");

            var appUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == userForLogin.Email);
            if (appUser == null)
                return Unauthorized("User not found in Users table");

            if (!appUser.UserId.HasValue)
                return StatusCode(500, "User ID is null");

            var token = _authHelper.CreateToken(appUser.UserId.Value);

            return Ok(new { token });
        }


        // -------------------- Refresh Token --------------------
        [Authorize]
        [HttpGet("RefreshToken")]
        public IActionResult RefreshToken()
        {
            string? userIdStr = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdStr))
                return Unauthorized();

            int userId = int.Parse(userIdStr);
            string token = _authHelper.CreateToken(userId);

            return Ok(new { token });
        }

        // -------------------- Reset Password --------------------
        [Authorize]
        [HttpPut("ResetPassword")]  // Using PUT for update semantics
        public async Task<IActionResult> ResetPassword([FromBody] LoginDto resetDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == resetDto.Email);
            if (user == null)
                return NotFound("User not found");

            if (!_authHelper.SetPassword(resetDto))
                return StatusCode(500, "Failed to reset password");

            return Ok("Password reset successfully");
        }
    }
}
