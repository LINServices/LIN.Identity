namespace LIN.Cloud.Identity.Services.Auth;


public class JwtService
{



    /// <summary>
    /// Llave del token
    /// </summary>
    private static string JwtKey { get; set; } = string.Empty;



    /// <summary>
    /// Inicia el servicio JwtService
    /// </summary>
    public static void Open()
    {
        JwtKey = Http.Services.Configuration.GetConfiguration("jwt:key");
    }




    /// <summary>
    /// Genera un JSON Web Token
    /// </summary>
    /// <param name="user">Modelo de usuario</param>
    internal static string Generate(AccountModel user, int appID)
    {

        if (JwtKey == string.Empty)
            Open();

        // Configuración
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));

        // Credenciales
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

        // Reclamaciones
        var claims = new[]
        {
            new Claim(ClaimTypes.PrimarySid, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Identity.Unique),
            new Claim(ClaimTypes.GroupSid, (user.Identity.Id).ToString() ?? ""),
            new Claim(ClaimTypes.Authentication, appID.ToString())
        };

        // Expiración del token
        var expiración = DateTime.Now.AddHours(5);

        // Token
        var token = new JwtSecurityToken(null, null, claims, null, expiración, credentials);

        // Genera el token
        return new JwtSecurityTokenHandler().WriteToken(token);
    }



    /// <summary>
    /// Valida un JSON Web token
    /// </summary>
    /// <param name="token">Token a validar</param>
    internal static JwtModel Validate(string token)
    {
        try
        {

            // Comprobación
            if (string.IsNullOrWhiteSpace(token))
                return new()
                {
                    IsAuthenticated = false
                };

            // Configurar la clave secreta
            var key = Encoding.ASCII.GetBytes(JwtKey);

            // Validar el token
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


                // Si el token es válido, puedes acceder a los claims (datos) del usuario
                var user = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

                // 
                _ = int.TryParse(jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.PrimarySid)?.Value, out var id);
                _ = int.TryParse(jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Authentication)?.Value, out var appID);
                _ = int.TryParse(jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.GroupSid)?.Value, out var identityId);


                // Devuelve una respuesta exitosa
                return new()
                {
                    IsAuthenticated = true,
                    AccountId = id,
                    ApplicationId = appID,
                    IdentityId = identityId,
                    Unique = user ?? ""
                };

            }
            catch (SecurityTokenException)
            {
            }


        }
        catch { }

        return new()
        {
            IsAuthenticated = false
        };

    }



}