using Auth.API.Data;
using Auth.API.Models;
using AuthAPI.Dtos;
using AuthAPI.Services;
using AuthAPI.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Auth.API.Tests.Services
{
    public class AuthAPIServiceLoginTests
    {
        private IdentityApplicationDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<IdentityApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new IdentityApplicationDbContext(options);
        }

        private async Task<ApplicationUser> CreateTestUserInDb(IdentityApplicationDbContext context, UserManager<ApplicationUser> userManager, 
            string username = "testuser", string email = "test@example.com", string password = "TestPassword123!")
        {
            var user = new ApplicationUser
            {
                UserName = username,
                Email = email,
                NormalizedEmail = email.ToUpper(),
                NormalizedUserName = username.ToUpper(),
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new Exception($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            return user;
        }

        private UserManager<ApplicationUser> CreateUserManager(IdentityApplicationDbContext context)
        {
            var store = new UserStore<ApplicationUser>(context);
            var passwordHasher = new PasswordHasher<ApplicationUser>();
            var logger = new Mock<ILogger<UserManager<ApplicationUser>>>().Object;
            
            var userManager = new UserManager<ApplicationUser>(
                store,
                null!,
                passwordHasher,
                null!,
                null!,
                null!,
                null!,
                null!,
                logger
            );
            return userManager;
        }

        [Fact]
        public async Task Login_WithValidCredentials_ShouldSucceed()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            var userManager = CreateUserManager(context);
            var mockJwtGenerator = new Mock<IJwtTokenGenerator>();

            const string password = "ValidPassword123!";
            var testUser = await CreateTestUserInDb(context, userManager, "testuser", "test@example.com", password);

            mockJwtGenerator
                .Setup(jg => jg.GenerateToken(testUser))
                .Returns("valid.jwt.token");

            var authService = new AuthAPIService(context, userManager, mockJwtGenerator.Object);

            var loginRequest = new LoginRequestDto
            {
                Username = "testuser",
                Password = password,
                RememberMe = false
            };

            // Act
            var response = await authService.Login(loginRequest);

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal("0", response.Code);
            Assert.NotNull(response.Result);
        }

        [Fact]
        public async Task Login_WithInvalidUsername_ShouldFail()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            var userManager = CreateUserManager(context);
            var mockJwtGenerator = new Mock<IJwtTokenGenerator>();

            var authService = new AuthAPIService(context, userManager, mockJwtGenerator.Object);

            var loginRequest = new LoginRequestDto
            {
                Username = "nonexistent",
                Password = "Password123!",
                RememberMe = false
            };

            // Act
            var response = await authService.Login(loginRequest);

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("-1", response.Code);
        }

        [Fact]
        public async Task Login_WithWrongPassword_ShouldFail()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            var userManager = CreateUserManager(context);
            var mockJwtGenerator = new Mock<IJwtTokenGenerator>();

            const string correctPassword = "CorrectPassword123!";
            await CreateTestUserInDb(context, userManager, "testuser", "test@example.com", correctPassword);

            var authService = new AuthAPIService(context, userManager, mockJwtGenerator.Object);

            var loginRequest = new LoginRequestDto
            {
                Username = "testuser",
                Password = "WrongPassword123!",
                RememberMe = false
            };

            // Act
            var response = await authService.Login(loginRequest);

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("-1", response.Code);
        }

        [Fact]
        public async Task Login_ShouldReturnJwtToken()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            var userManager = CreateUserManager(context);
            var mockJwtGenerator = new Mock<IJwtTokenGenerator>();

            const string password = "Password123!";
            var testUser = await CreateTestUserInDb(context, userManager, "testuser", "test@example.com", password);

            const string expectedToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.token.signature";
            mockJwtGenerator
                .Setup(jg => jg.GenerateToken(testUser))
                .Returns(expectedToken);

            var authService = new AuthAPIService(context, userManager, mockJwtGenerator.Object);

            var loginRequest = new LoginRequestDto
            {
                Username = "testuser",
                Password = password,
                RememberMe = false
            };

            // Act
            var response = await authService.Login(loginRequest);

            // Assert
            Assert.True(response.IsSuccess);
            var result = response.Result as LoginResponseDto;
            Assert.NotNull(result);
            Assert.Equal(expectedToken, result.Token);
        }

        [Fact]
        public async Task Login_ShouldReturnUserDetails()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            var userManager = CreateUserManager(context);
            var mockJwtGenerator = new Mock<IJwtTokenGenerator>();

            const string password = "Password123!";
            var testUser = await CreateTestUserInDb(context, userManager, "johndoe", "john@example.com", password);

            mockJwtGenerator
                .Setup(jg => jg.GenerateToken(testUser))
                .Returns("token.here");

            var authService = new AuthAPIService(context, userManager, mockJwtGenerator.Object);

            var loginRequest = new LoginRequestDto
            {
                Username = "johndoe",
                Password = password,
                RememberMe = false
            };

            // Act
            var response = await authService.Login(loginRequest);

            // Assert
            Assert.True(response.IsSuccess);
            var result = response.Result as LoginResponseDto;
            Assert.NotNull(result);
            Assert.NotNull(result.User);
            Assert.Equal("john@example.com", result.User.Email);
            Assert.Equal("johndoe", result.User.Name);
        }
    }
}
