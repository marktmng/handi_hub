using Microsoft.AspNetCore.Mvc;
using DotnetAPI.Data;
using DotnetAPI.Models;
using System.Threading.Tasks;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository; // Injected repository to access user data

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers(int? userId = null, string role = null)
        {
            var users = await _userRepository.GetUsersAsync(userId, role);
            return Ok(users);
        }

        [HttpPost("UpsertUser")]
        public async Task<IActionResult> UpsertUser([FromBody] User user)
        {
            if (user == null)
                return BadRequest("User is null.");

            var result = await _userRepository.UpsertUserAsync(user);
            if (result != null && result.UserId.HasValue)
                return Ok(result);

            return BadRequest("Failed to process user data.");
        }

        // New PUT endpoint for updating the full user info
        [HttpPut("Update/{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] User user)
        {
            if (user == null || user.UserId != userId)
                return BadRequest("User ID mismatch or user is null.");

            var updatedUser = await _userRepository.UpsertUserAsync(user);
            if (updatedUser != null && updatedUser.UserId.HasValue)
                return Ok(updatedUser);

            return BadRequest("Failed to update user.");
        }

        [HttpDelete("Delete/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var deleted = await _userRepository.DeleteUserAsync(userId);

            if (deleted)
                return Ok($"User with ID {userId} deleted successfully.");

            return NotFound($"User with ID {userId} was not found or already deleted.");
        }
    }
}
