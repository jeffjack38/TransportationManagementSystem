using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UserService.DTOs;
using UserService.Services;
using System.Security.Claims;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserServices _userService;

        public UserController(UserServices userService)
        {
            _userService = userService;
        }

        // 1. Update User Profile
        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _userService.UpdateUserProfileAsync(userId, model);

            if (!result)
            {
                return NotFound("User not found.");
            }

            return Ok("Profile updated successfully.");
        }

        // 2. Register a new user
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.RegisterUserAsync(model);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("User registered successfully.");
        }

        // 3. User Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var token = await _userService.LoginUserAsync(model);

            if (token == null)
            {
                return Unauthorized("Invalid login attempt.");
            }

            return Ok(new { Token = token });
        }

        // 4. Get Current User Info
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(user);
        }

        // 5. Reset Password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.ResetPasswordAsync(model);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("Password reset successfully.");
        }

        // 6. Delete User Account
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userService.DeleteUserAsync(id);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("User deleted successfully.");
        }
    }
}
