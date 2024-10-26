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
        // dependencies
        private readonly UserManager<User> _userManager; // manages user creation, deletion, searching, and updating
        private readonly SignInManager<User> _signInManager; // authentication (sign in, sign out)
        private readonly RoleManager<IdentityRole> _roleManager; // manages role creation, deletion, searching, and updating
        private readonly UserDbContext _context; 
        private readonly IConfiguration _configuration; // provides access to app settings

        // constructor - injects dependencies
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

        //REGISTER
        //method to register a new user, accepts a RegisterViewModel object, returns an IdentityResult
        public async Task<IdentityResult> RegisterUserAsync(RegisterViewModel model)
        {
            // initialize User object with data from RegisterViewModel
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

            // CreateAsync will automatically hash the password, then uses Entity FC to save the User object to the AspNetUsers table 
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
               
                if (!await _roleManager.RoleExistsAsync(model.Role))
                {
                    return IdentityResult.Failed(new IdentityError { Description = $"Role '{model.Role}' does not exist." });
                }

                
                await _userManager.AddToRoleAsync(user, model.Role);
            }

            return result; // returns the IdentityResult from CreateAsync
        }


        //LOGIN
        //method to authenticate a user, accepts a LoginViewModel object, returns a JWT token or null
        //<string> represents the JWT token or null if login fails
        public async Task<string> LoginUserAsync(LoginViewModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return null;
            }

            //retrieves the User object for JWT creation
            var user = await _userManager.FindByEmailAsync(model.Email);
            //calls GenerateJwtToken to create the JWT token containing user-specific claims - return the genrated JWT token to allow the client to use it for authenticated requests
            return await GenerateJwtToken(user);  
        }

        
        public async Task<User> GetUserByIdAsync(string userId)
        {
            //FindByIdAsync - built in method of UserManager, takes userId as a paramter, searches for a matching user in the db
            //asynchronous so it doesnt block the trhead while it interacts with the db, returns a Task<User> object, return null if no user is found
            return await _userManager.FindByIdAsync(userId);
        }

        //GETALLUSERSWITHROLES
        //method to retrieve all users from the db along with their roles, map to UserDTO objects, return collection of UserDTO objects
        public async Task<IEnumerable<UserDTO>> GetAllUsersWithRolesAsync()
        {
            //user instance _userManager of UserManager<User>, User property retrieve all users as an IQueryable<User> object from the AspNetUsers table
            //then use .ToList() to convert IQueryable<User> to a List<User> to execute the query and retrieve all users from the db
            //this only retrieves the users, not their roles
            var users = _userManager.Users.ToList();

            //initialize userDTO Collection - create an empty list of UserDTO objects
            var userDTOs = new List<UserDTO>();

            //loop through users and Retrive Roles
            foreach (var user in users)
            {
                //GetRoleAsync method, asynchronous provided by UserManager to retrieve the roles for a user
                //takes a User object as a parameter, returns a List<string> of role names
                //use await, retreiving roels involve db access
                var roles = await _userManager.GetRolesAsync(user); 

                //creaet a UserDTO instance
                var userDTO = new UserDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    IsActive = user.IsActive,
                    LastLoginDate = user.LastLoginDate,
                    Role = roles.FirstOrDefault() //asign the first role to the Role property or only role
                };

                userDTOs.Add(userDTO);
            }

            return userDTOs; //return list of UserDTO objects
        }

        //RESETPASSWORD
        //find user by email, verify user existence, generate password reset token, reset password, return IdentityResult
        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            //FindByEmailAsync - built in method of UserManager, takes email as a paramter, searches for a matching user in the db
            var user = await _userManager.FindByEmailAsync(model.Email);

            //check if user is null - if user is not found, return IdentityResult with an error message
            if (user == null)
            {
                //IdentityResult - static method from ASP.NET Idenity, returns an IdentityResult that indicates failure
                //IdentityError - class that represents an error that occurred during an identity operation
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }

            //if user exists method to generate a secure password reset token - token one-time-use string that authorizes the user to reset their password.
            //crytpographically secure, expires after a set period of time, can only be used by the user who requested it
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            //ResetPasswordAsync - built in method of UserManager, takes user, token, and new password as parameters - returns an IdentityResult
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            return result; //returns the IdentityResult from ResetPasswordAsync
        }

        //DELETE
        //find user by id, verify user existence, delete user, return IdentityResult
        public async Task<IdentityResult> DeleteUserAsync(string userId)
        {
            //Find User by Id - UserManager class proveds method, FindByIdAsync that finds the user by unique ID
            //searches for userId in the AspNetUsers table, returns a Task<User> object, returns null if no user is found
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }

            return await _userManager.DeleteAsync(user);
        }

        
        //UPDATE
        //find user by id, verify if user exists, update user properties, save changes with UpdateAsync, return success status
        public async Task<bool> UpdateUserProfileAsync(string userId, UpdateProfileViewModel model)
        {
            //find the user by id, FindByIdAsync - searches the AspNetUsers table for a user with a matching ID, returns a Task<User> object
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
            user.Role = model.Role;

            //UpdateAsync - built in method of UserManager, takes a User object as a parameter, updates the user in the db
            var result = await _userManager.UpdateAsync(user); 

            return result.Succeeded; //return bool whether update was successful
        }

        //JWT TOKEN
        //initialize token handler and key, define claims, add role claims, configure token descriptor, create and return the JWT
        private async Task<string> GenerateJwtToken(User user)
        {
            //tokenHandler - instance of JwtSecurityTokenHandler class, handles the process of token creation and provides methods CreateToken and WriteToken
            var tokenHandler = new JwtSecurityTokenHandler();

            //key - retrieve the JWT key from the appsettings.json file, convert the key to a byte array
            //Encoding.UTF8.GetBytes - converts the string to a byte array - required to generate a SymmetricSecurityKey for token signing
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            //claims - piece of info about the user embedded within the JWT - carry user data in the token that can be read and verified without accessing the db
            var claims = new List<Claim>
            {
                //represents the user's unique identifeir - set to user.Id
                new Claim(JwtRegisteredClaimNames.Sub, user.Id), 
                //represents the user's email - set to user.Email
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                //represents the user's unique identifier - set to user.Id
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            //add user roles as claims 
            //retrieve all roles assigned to the user from _userManager
            var roles = await _userManager.GetRolesAsync(user); 

            //loop through the roles and add each role as a claim
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            //SecurityTokenDescriptor - object that contains the token's claims, expiration time, and signing credentials
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //Subject - represents the user's claims, set to the claims list
                Subject = new ClaimsIdentity(claims),
                //Expires - represents the token's expiration time, set to 1 hour from the current time
                Expires = DateTime.UtcNow.AddHours(1),
                //SigningCredentials - how the token will be signed, SymmetricSecurityKey - key used to sign the token, SecurityAlgorithms.HmacSha256Signature -
                //algorithm used to sign the token
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                //Audience - represents the recipient of the token, set to the value from the appsettings.json file
                Audience = _configuration["Jwt:Audience"],
                //Issuer - represents the token's issuer, set to the value from the appsettings.json file
                Issuer = _configuration["Jwt:Issuer"]
            };

            //create the JWT token using the tokenHandler and tokenDescriptor
            var token = tokenHandler.CreateToken(tokenDescriptor);
            //WriteToken - serialized the JwtSecurityToken into a URL-safe string
            //returns the JWT token as a string that can be used in the [Authorize] header of HTTP request to authorize access to protected resources
            return tokenHandler.WriteToken(token);
        }


    }
}
