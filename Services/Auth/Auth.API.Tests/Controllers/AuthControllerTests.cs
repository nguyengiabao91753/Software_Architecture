using AuthAPI.Controllers;
using AuthAPI.Dtos;
using AuthAPI.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Auth.API.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthAPIService> _mockAuthService;
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthAPIService>();
            _authController = new AuthController(_mockAuthService.Object);
        }

        [Fact]
        public async Task Register_WithValidRequest_ShouldReturnOkResult()
        {
            // Arrange
            var registerRequest = new RegistrationRequestDto
            {
                UserName = "newuser",
                Email = "new@example.com",
                PhoneNumber = "0123456789",
                Password = "Password123",
                ConfirmPassword = "Password123"
            };

            var successResponse = new ResponseDto
            {
                IsSuccess = true,
                Code = "0",
                Message = "User Registration Successful",
                Result = "user-id-123"
            };

            _mockAuthService
                .Setup(s => s.Register(registerRequest))
                .ReturnsAsync(successResponse);

            // Act
            var result = await _authController.Register(registerRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnedResponse = okResult.Value as ResponseDto;
            Assert.NotNull(returnedResponse);
            Assert.True(returnedResponse.IsSuccess);
        }

        [Fact]
        public async Task Register_WithInvalidRequest_ShouldReturnBadRequest()
        {
            // Arrange
            var registerRequest = new RegistrationRequestDto
            {
                UserName = "user",
                Email = "invalid-email",
                PhoneNumber = "123",
                Password = "weak",
                ConfirmPassword = "weak"
            };

            var errorResponse = new ResponseDto
            {
                IsSuccess = false,
                Code = "-1",
                Message = "Email is invalid",
                Result = null
            };

            _mockAuthService
                .Setup(s => s.Register(registerRequest))
                .ReturnsAsync(errorResponse);

            // Act
            var result = await _authController.Register(registerRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            var returnedResponse = badRequestResult.Value as ResponseDto;
            Assert.NotNull(returnedResponse);
            Assert.False(returnedResponse.IsSuccess);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnOkResult()
        {
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                Username = "testuser",
                Password = "ValidPassword123",
                RememberMe = false
            };

            var loginResponse = new LoginResponseDto
            {
                User = new UserDto
                {
                    ID = "user-123",
                    Name = "testuser",
                    Email = "test@example.com",
                    PhoneNumber = "0123456789"
                },
                Token = "valid.jwt.token"
            };

            var successResponse = new ResponseDto
            {
                IsSuccess = true,
                Code = "0",
                Message = "Login Successful",
                Result = loginResponse
            };

            _mockAuthService
                .Setup(s => s.Login(loginRequest))
                .ReturnsAsync(successResponse);

            // Act
            var result = await _authController.Login(loginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnedResponse = okResult.Value as ResponseDto;
            Assert.NotNull(returnedResponse);
            Assert.True(returnedResponse.IsSuccess);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                Username = "testuser",
                Password = "WrongPassword",
                RememberMe = false
            };

            var errorResponse = new ResponseDto
            {
                IsSuccess = false,
                Code = "-1",
                Message = "username or password incorrect.",
                Result = null
            };

            _mockAuthService
                .Setup(s => s.Login(loginRequest))
                .ReturnsAsync(errorResponse);

            // Act
            var result = await _authController.Login(loginRequest);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
            var returnedResponse = unauthorizedResult.Value as ResponseDto;
            Assert.NotNull(returnedResponse);
            Assert.False(returnedResponse.IsSuccess);
        }

        [Fact]
        public async Task Register_ShouldCallAuthServiceRegister()
        {
            // Arrange
            var registerRequest = new RegistrationRequestDto
            {
                UserName = "testuser",
                Email = "test@example.com",
                PhoneNumber = "0123456789",
                Password = "Password123",
                ConfirmPassword = "Password123"
            };

            var response = new ResponseDto
            {
                IsSuccess = true,
                Code = "0",
                Message = "User Registration Successful",
                Result = "user-id"
            };

            _mockAuthService
                .Setup(s => s.Register(registerRequest))
                .ReturnsAsync(response);

            // Act
            await _authController.Register(registerRequest);

            // Assert
            _mockAuthService.Verify(s => s.Register(registerRequest), Times.Once);
        }

        [Fact]
        public async Task Login_ShouldCallAuthServiceLogin()
        {
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                Username = "testuser",
                Password = "Password123",
                RememberMe = false
            };

            var response = new ResponseDto
            {
                IsSuccess = true,
                Code = "0",
                Message = "Login Successful",
                Result = new LoginResponseDto
                {
                    User = new UserDto { ID = "user-123", Name = "testuser" },
                    Token = "jwt.token"
                }
            };

            _mockAuthService
                .Setup(s => s.Login(loginRequest))
                .ReturnsAsync(response);

            // Act
            await _authController.Login(loginRequest);

            // Assert
            _mockAuthService.Verify(s => s.Login(loginRequest), Times.Once);
        }

        [Fact]
        public async Task Register_WithEmptyRequest_ShouldReturnBadRequest()
        {
            // Arrange
            var registerRequest = new RegistrationRequestDto
            {
                UserName = "",
                Email = "",
                PhoneNumber = "",
                Password = "",
                ConfirmPassword = ""
            };

            var errorResponse = new ResponseDto
            {
                IsSuccess = false,
                Code = "-1",
                Message = "Username and email are required",
                Result = null
            };

            _mockAuthService
                .Setup(s => s.Register(registerRequest))
                .ReturnsAsync(errorResponse);

            // Act
            var result = await _authController.Register(registerRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }
    }
}
