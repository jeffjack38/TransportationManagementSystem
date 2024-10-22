using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UserService.DTOs;
using SharedModels.Models;
using UserService.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace UserService.Services
{
    public class UserServices
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserDbContext _context;
        private readonly IConfiguration _configuration;


        public UserServices(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager,
            UserDbContext context,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
            _configuration = configuration;
        }

        
        public async Task<IdentityResult> RegisterUserAsync(RegisterViewModel model)
        {
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                City = model.City,
                State = model.State,
                ZipCode = model.ZipCode,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
               
                if (!await _roleManager.RoleExistsAsync(model.Role))
                {
                    return IdentityResult.Failed(new IdentityError { Description = $"Role '{model.Role}' does not exist." });
                }

                
                await _userManager.AddToRoleAsync(user, model.Role);
            }

            return result;
        }

        
        public async Task<string> LoginUserAsync(LoginViewModel model)
{
    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

    if (!result.Succeeded)
    {
        return null;
    }

    var user = await _userManager.FindByEmailAsync(model.Email);
    return await GenerateJwtToken(user);  
}

        
        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        
        public async Task<IEnumerable<UserDTO>> GetAllUsersWithRolesAsync()
        {
            var users = _userManager.Users.ToList();
            var userDTOs = new List<UserDTO>();

            foreach (var user in users)
            {
                
                var roles = await _userManager.GetRolesAsync(user);

                var userDTO = new UserDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    IsActive = user.IsActive,
                    LastLoginDate = user.LastLoginDate,
                    Role = roles.FirstOrDefault()
                };

                userDTOs.Add(userDTO);
            }

            return userDTOs;
        }


        
        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            return result;
        }

        
        public async Task<IdentityResult> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }

            return await _userManager.DeleteAsync(user);
        }

        
        public async Task<bool> UpdateUserProfileAsync(string userId, UpdateProfileViewModel model)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false; 
            }

            
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Address = model.Address;
            user.City = model.City;
            user.State = model.State;
            user.ZipCode = model.ZipCode;
            user.PhoneNumber = model.PhoneNumber;
            user.LastProfileUpdate = DateTime.UtcNow; 

            var result = await _userManager.UpdateAsync(user); 

            return result.Succeeded; 
        }

        
        private async Task<string> GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var roles = await _userManager.GetRolesAsync(user); 
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = _configuration["Jwt:Audience"],
                Issuer = _configuration["Jwt:Issuer"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


    }
}
