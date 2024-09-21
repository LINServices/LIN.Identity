using LIN.Cloud.Identity.Services.Auth;
using LIN.Types.Cloud.Identity.Models;

namespace LIN.Identity.Tests.Auth
{
    public class JwtServiceTests
    {
        private const string TestJwtKey = "wTgdLCN4GuV37K5I1H6C331Z146tylLINkiZt69nokIdwTgdLCN4GuV37K5I1H6C331Z146tylLINkiZt69nokId";

        public JwtServiceTests()
        {
            // Configurar la llave JWT para las pruebas
            typeof(JwtService).GetProperty("JwtKey", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).SetValue(null, TestJwtKey);
        }

        [Fact]
        public void Generate_ShouldReturnValidToken()
        {
            // Arrange
            var user = new AccountModel
            {
                Id = 1,
                Identity = new IdentityModel { Unique = "unique_user" }
            };
            int appID = 123;

            // Act
            var token = JwtService.Generate(user, appID);

            // Assert
            Assert.False(string.IsNullOrEmpty(token));
        }

        [Fact]
        public void Validate_ShouldReturnAuthenticatedModel_WhenTokenIsValid()
        {
            // Arrange
            var user = new AccountModel
            {
                Id = 1,
                Identity = new IdentityModel { Unique = "unique_user" }
            };
            int appID = 123;
            var token = JwtService.Generate(user, appID);

            // Act
            var result = JwtService.Validate(token);

            // Assert
            Assert.True(result.IsAuthenticated);
            Assert.Equal(user.Id, result.AccountId);
            Assert.Equal(user.Identity.Unique, result.Unique);
            Assert.Equal(appID, result.ApplicationId);
        }

        [Fact]
        public void Validate_ShouldReturnNotAuthenticatedModel_WhenTokenIsInvalid()
        {
            // Arrange
            var invalidToken = "invalid_token";

            // Act
            var result = JwtService.Validate(invalidToken);

            // Assert
            Assert.False(result.IsAuthenticated);
        }

    }
}