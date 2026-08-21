using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LIN.Cloud.Identity.Services.Services;

public class JwtApplicationsService
{

    /// <summary>
    /// Llave del token
    /// </summary>
    private static string JwtKey { get; set; } = string.Empty;


    /// <summary>
    /// Inicia el servicio JwtService
    /// </summary>
    public static void Open(IConfiguration configuration)
    {
        JwtKey = configuration["jwt:keyapp"] ?? string.Empty;
    }


    /// <summary>
    /// Genera un JSON Web Token
    /// </summary>
    /// <param name="appID">Id de la aplicación.</param>
    public static string Generate(int appID)
    {

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

        var claims = new[]
        {
            new Claim(ClaimTypes.Authentication, appID.ToString())
        };

        var expiración = DateTime.UtcNow.AddMinutes(5);
        var token = new JwtSecurityToken(null, null, claims, null, expiración, credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    /// <summary>
    /// Valida un JSON Web token
    /// </summary>
    /// <param name="token">Token a validar</param>
    public static int Validate(string token)
    {
        try
        {

            if (string.IsNullOrWhiteSpace(token))
                return 0;

            var key = Encoding.ASCII.GetBytes(JwtKey);
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = true
            };

            try
            {

                var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;

                var user = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
                _ = int.TryParse(jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Authentication)?.Value, out var appID);

                return appID;
            }
            catch (SecurityTokenException)
            {
            }


        }
        catch { }

        return 0;

    }


}