using Auth.API.Models;
using AuthAPI.Services.IServices;
using Microsoft.Extensions.Options;
using Moq;
using SecShare.Servicer.Auth;
using System.IdentityModel.Tokens.Jwt;
using Xunit;

namespace Auth.API.Tests.Services
{
    public class JwtTokenGeneratorTests
    {
        private readonly JwtOptions _jwtOptions;
        private readonly JwtTokenGenerator _tokenGenerator;

        public JwtTokenGeneratorTests()
        {
            _jwtOptions = new JwtOptions
            {
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                Secret = "ThisIsAVerySecretKeyThatIsAtLeast32CharactersLongForTestingPurposes"
            };

            var mockOptions = new Mock<IOptions<JwtOptions>>();
            mockOptions.Setup(o => o.Value).Returns(_jwtOptions);

            _tokenGenerator = new JwtTokenGenerator(mockOptions.Object);
        }

        private ApplicationUser CreateTestUser(string id = "user-123", string username = "testuser", string email = "test@example.com")
        {
            return new ApplicationUser
            {
                Id = id,
                UserName = username,
                Email = email,
                PhoneNumber = "0123456789"
            };
        }

        [Fact]
        public void GenerateToken_WithValidUser_ShouldCreateValidToken()
        {
            // Arrange
            var user = CreateTestUser();

            // Act
            var token = _tokenGenerator.GenerateToken(user);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);
            Assert.Contains(".", token); // JWT tokens have dots separating parts
        }

        [Fact]
        public void GenerateToken_ShouldReturnJwtFormat()
        {
            // Arrange
            var user = CreateTestUser();

            // Act
            var token = _tokenGenerator.GenerateToken(user);

            // Assert
            var parts = token.Split('.');
            Assert.Equal(3, parts.Length); // JWT should have 3 parts: header.payload.signature
            Assert.NotEmpty(parts[0]); // Header
            Assert.NotEmpty(parts[1]); // Payload
            Assert.NotEmpty(parts[2]); // Signature
        }

        [Fact]
        public void GenerateToken_ShouldContainUserEmailClaim()
        {
            // Arrange
            var user = CreateTestUser(email: "johndoe@example.com");

            // Act
            var token = _tokenGenerator.GenerateToken(user);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email);

            Assert.NotNull(emailClaim);
            Assert.Equal("johndoe@example.com", emailClaim.Value);
        }

        [Fact]
        public void GenerateToken_ShouldContainUserIdClaim()
        {
            // Arrange
            var user = CreateTestUser(id: "specific-user-id-789");

            // Act
            var token = _tokenGenerator.GenerateToken(user);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var subClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);

            Assert.NotNull(subClaim);
            Assert.Equal("specific-user-id-789", subClaim.Value);
        }

        [Fact]
        public void GenerateToken_ShouldContainUsernameClaim()
        {
            // Arrange
            var user = CreateTestUser(username: "johndoe");

            // Act
            var token = _tokenGenerator.GenerateToken(user);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var nameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name);

            Assert.NotNull(nameClaim);
            Assert.Equal("johndoe", nameClaim.Value);
        }

        [Fact]
        public void GenerateToken_ShouldContainAllRequiredClaims()
        {
            // Arrange
            var user = CreateTestUser(id: "user-001", username: "alice", email: "alice@example.com");

            // Act
            var token = _tokenGenerator.GenerateToken(user);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            Assert.Contains(jwtToken.Claims, c => c.Type == JwtRegisteredClaimNames.Email && c.Value == "alice@example.com");
            Assert.Contains(jwtToken.Claims, c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == "user-001");
            Assert.Contains(jwtToken.Claims, c => c.Type == JwtRegisteredClaimNames.Name && c.Value == "alice");
        }

        [Fact]
        public void GenerateToken_TokenShouldHaveCorrectIssuer()
        {
            // Arrange
            var user = CreateTestUser();

            // Act
            var token = _tokenGenerator.GenerateToken(user);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            Assert.Equal(_jwtOptions.Issuer, jwtToken.Issuer);
        }

        [Fact]
        public void GenerateToken_TokenShouldHaveCorrectAudience()
        {
            // Arrange
            var user = CreateTestUser();

            // Act
            var token = _tokenGenerator.GenerateToken(user);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            Assert.Contains(_jwtOptions.Audience, jwtToken.Audiences);
        }

        [Fact]
        public void GenerateToken_TokenShouldHaveExpirationDate()
        {
            // Arrange
            var user = CreateTestUser();
            var beforeGeneration = DateTime.UtcNow;

            // Act
            var token = _tokenGenerator.GenerateToken(user);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            Assert.True(jwtToken.ValidTo > beforeGeneration);
            // Token should expire in approximately 30 days
            var expectedExpiration = beforeGeneration.AddDays(30);
            var difference = Math.Abs((jwtToken.ValidTo - expectedExpiration).TotalSeconds);
            Assert.True(difference < 60); // Within 60 seconds
        }

        [Fact]
        public void GenerateToken_DifferentUsers_ShouldGenerateDifferentTokens()
        {
            // Arrange
            var user1 = CreateTestUser(id: "user-1", username: "alice");
            var user2 = CreateTestUser(id: "user-2", username: "bob");

            // Act
            var token1 = _tokenGenerator.GenerateToken(user1);
            var token2 = _tokenGenerator.GenerateToken(user2);

            // Assert
            Assert.NotEqual(token1, token2);
        }

    }
}
