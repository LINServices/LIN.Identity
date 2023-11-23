﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LIN.Identity.Services;


public class Jwt
{


    /// <summary>
    /// Llave del token
    /// </summary>
    private static string JwtKey { get; set; } = string.Empty;



    /// <summary>
    /// Inicia el servicio Jwt
    /// </summary>
    public static void Open()
    {
        JwtKey = Configuration.GetConfiguration("jwt:key");
    }




    /// <summary>
    /// Genera un JSON Web Token
    /// </summary>
    /// <param name="user">Modelo de usuario</param>
    internal static string Generate(AccountModel user, int appID)
    {

        // Configuración

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));

        // Credenciales
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

        // Reclamaciones
        var claims = new[]
        {
            new Claim(ClaimTypes.PrimarySid, user.ID.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Usuario),
            new Claim(ClaimTypes.Role, ((int)user.Rol).ToString()),
            new Claim(ClaimTypes.UserData, (user.OrganizationAccess?.Organization.ID).ToString() ?? ""),
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
    internal static (bool isValid, string user, int userID, int orgID, int appID) Validate(string token)
    {
        try
        {

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

                _ = int.TryParse(jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.UserData)?.Value, out var orgID);

                _ = int.TryParse(jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Authentication)?.Value, out var appID);


                // Devuelve una respuesta exitosa
                return (true, user ?? string.Empty, id, orgID, appID);

            }
            catch (SecurityTokenException)
            {
            }


        }
        catch { }

        return (false, string.Empty, 0, 0, 0);

    }


}