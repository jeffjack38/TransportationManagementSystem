using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;
using UserService.Services;
using UserService.DTOs;
using SharedModels.Models;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;


namespace UserService.Tests
{

    public class UserServicesTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;
        private readonly Mock<SignInManager<User>> _mockSignInManager;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly UserServices _userService;

        public UserServicesTests()
        {
            _mockUserManager = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            _mockRoleManager = new Mock<RoleManager<IdentityRole>>(
                Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);

            _mockSignInManager = new Mock<SignInManager<User>>(
                _mockUserManager.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<User>>(),
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<ILogger<SignInManager<User>>>(),
                Mock.Of<IAuthenticationSchemeProvider>(),
                Mock.Of<IUserConfirmation<User>>());

            // Mock IConfiguration
            _mockConfiguration = new Mock<IConfiguration>();

            // Setup mock JWT configuration values
            _mockConfiguration.Setup(x => x["Jwt:Key"]).Returns("c2VjdXJlLWhtYWMtc2hhMjU2LWtleQ=="); //256-bit key
            _mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("yourdomain.com");
            _mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("yourdomain.com");


            // Pass _mockSignInManager.Object instead of null
            _userService = new UserServices(_mockUserManager.Object, _mockSignInManager.Object, _mockRoleManager.Object, null, _mockConfiguration.Object);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnSucceeded_WhenUserIsCreated()
        {
            // Arrange
            var registerViewModel = new RegisterViewModel
            {
                Email = "testuser@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                FirstName = "Test",
                LastName = "User",
                Address = "123 Main St",
                City = "Sample City",
                State = "Sample State",
                ZipCode = "12345"
            };

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                            .ReturnsAsync(IdentityResult.Success);

            _mockRoleManager.Setup(x => x.RoleExistsAsync(It.IsAny<string>()))
                            .ReturnsAsync(false);

            // Act
            var result = await _userService.RegisterUserAsync(registerViewModel);

            // Assert
            result.Succeeded.Should().BeTrue();
            _mockUserManager.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
            _mockRoleManager.Verify(x => x.RoleExistsAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnFailed_WhenUserCreationFails()
        {
            // Arrange
            var registerViewModel = new RegisterViewModel
            {
                Email = "testuser@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                FirstName = "Test",
                LastName = "User",
                Address = "123 Main St",
                City = "Sample City",
                State = "Sample State",
                ZipCode = "12345"
            };

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error creating user." }));

            // Act
            var result = await _userService.RegisterUserAsync(registerViewModel);

            // Assert
            result.Succeeded.Should().BeFalse();
        }

        [Fact]
        public async Task LoginUserAsync_ShouldReturnToken_WhenLoginIsSuccessful()
        {
            // Arrange
            var loginViewModel = new LoginViewModel
            {
                Email = "testuser@example.com",
                Password = "Password123!"
            };

            var user = new User { UserName = "testuser@example.com", Email = "testuser@example.com" };
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                            .ReturnsAsync(user);  // Simulate a user being found

            // Mock SignInManager.PasswordSignInAsync to return Success
            var mockSignInResult = SignInResult.Success;
            _mockSignInManager.Setup(x => x.PasswordSignInAsync(
                It.Is<string>(email => email == loginViewModel.Email),       // Email check
                It.Is<string>(password => password == loginViewModel.Password),  // Password check
                It.IsAny<bool>(),    // isPersistent (false in this case)
                It.IsAny<bool>()))   // lockoutOnFailure (false in this case)
                .ReturnsAsync(mockSignInResult);  // Simulate successful sign-in

            // Act
            var result = await _userService.LoginUserAsync(loginViewModel);

            // Assert
            result.Should().NotBeNull(); // The JWT token should not be null on successful login
        }

        [Fact]
        public async Task LoginUserAsync_ShouldReturnNull_WhenLoginFails()
        {
            // Arrange
            var loginViewModel = new LoginViewModel
            {
                Email = "testuser@example.com",
                Password = "WrongPassword123!"
            };

            var user = new User { UserName = "testuser@example.com", Email = "testuser@example.com" };
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                            .ReturnsAsync(user);  // Simulate a user being found

            // Mock SignInManager.PasswordSignInAsync to return Failed
            var mockSignInResult = SignInResult.Failed;
            _mockSignInManager.Setup(x => x.PasswordSignInAsync(
                It.Is<string>(email => email == loginViewModel.Email),       // Email check
                It.Is<string>(password => password == loginViewModel.Password),  // Password check
                It.IsAny<bool>(),    // isPersistent (false in this case)
                It.IsAny<bool>()))   // lockoutOnFailure (false in this case)
                .ReturnsAsync(mockSignInResult);  // Simulate failed sign-in

            // Act
            var result = await _userService.LoginUserAsync(loginViewModel);

            // Assert
            result.Should().BeNull(); // The JWT token should be null on failed login
        }

        [Fact]
        public async Task UpdateUserProfileAsync_ShouldUpdateProfile_WhenUserExists()
        {
            // Arrange
            var userId = "123"; // Simulate a user ID
            var updateProfileViewModel = new UpdateProfileViewModel
            {
                FirstName = "John",
                LastName = "Doe",
                Address = "123 Main St",
                City = "Sample City",
                State = "Sample State",
                ZipCode = "12345",
                PhoneNumber = "555-555-5555"
            };

            var user = new User { Id = userId, UserName = "testuser@example.com", Email = "testuser@example.com" };

            // Mock UserManager to return an existing user
            _mockUserManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.UpdateUserProfileAsync(userId, updateProfileViewModel);

            // Assert
            result.Should().BeTrue(); // Ensure the profile update was successful
            _mockUserManager.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task ResetPasswordAsync_ShouldResetPassword_WhenTokenIsValid()
        {
            // Arrange
            var resetPasswordViewModel = new ResetPasswordViewModel
            {
                Email = "testuser@example.com",
                NewPassword = "NewPassword123!",
                ConfirmPassword = "NewPassword123!"
            };

            var user = new User { UserName = "testuser@example.com", Email = "testuser@example.com" };

            // Mock UserManager to return an existing user
            _mockUserManager.Setup(x => x.FindByEmailAsync(resetPasswordViewModel.Email)).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.GeneratePasswordResetTokenAsync(user)).ReturnsAsync("valid_token");
            _mockUserManager.Setup(x => x.ResetPasswordAsync(user, "valid_token", resetPasswordViewModel.NewPassword)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.ResetPasswordAsync(resetPasswordViewModel);

            // Assert
            result.Succeeded.Should().BeTrue(); // Ensure the password reset was successful
            _mockUserManager.Verify(x => x.ResetPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldDeleteUser_WhenUserExists()
        {
            // Arrange
            var userId = "123";
            var user = new User { Id = userId, UserName = "testuser@example.com", Email = "testuser@example.com" };

            // Mock UserManager to return an existing user
            _mockUserManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.DeleteUserAsync(userId);

            // Assert
            result.Succeeded.Should().BeTrue(); // Ensure the user deletion was successful
            _mockUserManager.Verify(x => x.DeleteAsync(It.IsAny<User>()), Times.Once);
        }





    }
}