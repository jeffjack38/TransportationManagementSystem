using Microsoft.AspNetCore.Authorization; // attributes and classes for role-based authorization
using Microsoft.AspNetCore.Mvc; //classes for creating MVC controllers
using System.Threading.Tasks; 
using UserService.DTOs;
using UserService.Services;
using System.Security.Claims; // classes for working with claims-based identity, retrieving the authenticated user's ID

namespace UserService.Controllers
{
    // [ApiController] provides model statet validation and error responses
    [ApiController]
    [Route("api/[controller]")] 
    public class UserController : ControllerBase
    {
        private readonly UserServices _userService;

        //constructor to inject UserServices
        public UserController(UserServices userService)
        {
            _userService = userService;
        }

        // UPDATE PROFILE - PUT: api/user/profile
        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileViewModel model) //model binding, binds data from the request body and validates it against the UpdateProfileViewModel's attributes
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //retrive the currently authenticated user's ID from claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _userService.UpdateUserProfileAsync(userId, model);

            if (!result)
            {
                return NotFound("User not found.");
            }

            return Ok("Profile updated successfully.");
        }

        // POST: api/user/register
        //Only Admin role can access this endpoint, when user is onboarded Admin will register and give credentials to user
        [Authorize(Roles = "Admin")]  
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

        // POST: api/user/login
        //Login endpoint for user to authenticate and get JWT token
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

        // GET: api/user/me
        // Get current user details
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

        // POST: api/user/reset-password
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

        // DELETE: api/user/{id}
        // Admin only endpoint
        [Authorize(Roles = "Admin")]
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

        // GET: api/user/all-users
        // Admin only endpoint
        [Authorize(Roles = "Admin")]  
        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersWithRolesAsync();
            if (users == null || !users.Any())
            {
                return NotFound("No users found.");
            }

            return Ok(users);
        }
    }
}
