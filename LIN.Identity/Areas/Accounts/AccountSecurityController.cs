namespace LIN.Identity.Areas.Accounts;


[Route("account/security")]
public class AccountSecurityController : ControllerBase
{



    /// <summary>
    /// Elimina una cuenta
    /// </summary>
    /// <param name="token">Token de acceso</param>
    [HttpDelete("delete")]
    public async Task<HttpResponseBase> Delete([FromHeader] string token)
    {

        // Información del token.
        var (isValid, _, userId, _, _) = Jwt.Validate(token);

        // Si es invalido.
        if (!isValid)
            return new ResponseBase
            {
                Response = Responses.Unauthorized,
                Message = "Token invalido"
            };

        if (userId <= 0)
            return new(Responses.InvalidParam);

        var response = await Data.Accounts.Delete(userId);
        return response;
    }



    /// <summary>
    /// Actualizar la contraseña.
    /// </summary>
    /// <param name="account">Id de la cuenta actual.</param>
    /// <param name="actualPassword">Contraseña actual.</param>
    /// <param name="newPassword">Nueva contraseña.</param>
    [HttpPatch("update/password")]
    public async Task<HttpResponseBase> UpdatePassword([FromHeader] int account, [FromQuery] string actualPassword, [FromHeader] string newPassword)
    {

        // Validación de parámetros.
        if (account <= 0 || string.IsNullOrWhiteSpace(actualPassword) || string.IsNullOrWhiteSpace(newPassword))
            return new(Responses.InvalidParam)
            {
                Message = "Uno o varios parámetros son inválidos."
            };

        // Tamaño invalido.
        if (newPassword.Length < 4)
            return new(Responses.InvalidParam)
            {
                Message = "La nueva contraseña debe de tener mas de 4 dígitos."
            };


        // Data actual.
        var actualData = await Data.Accounts.Read(account, new()
        {
            ContextOrg = 0,
            SensibleInfo = true,
            ContextUser = account,
            FindOn = FilterModels.FindOn.StableAccounts,
            IncludeOrg = FilterModels.IncludeOrg.None,
            IsAdmin = true,
            OrgLevel = FilterModels.IncludeOrgLevel.Basic
        });

        // Si no existe la cuenta.
        if (actualData.Response != Responses.Success)
            return new(Responses.NotExistAccount)
            {
                Message = "No se encontró la cuenta."
            };

        // Encriptar la contraseña actual.
        if (EncryptClass.Encrypt(actualPassword) != actualData.Model.Contraseña)
            return new(Responses.Unauthorized)
            {
                Message = "La contraseña actual es diferente a la proporcionada."
            };

        // Validar políticas de contraseña.
        var result = await AccountPassword.ValidatePassword(account, newPassword);

        // Invalido por políticas
        if (!result)
            return new(Responses.PoliciesNotComplied)
            {
                Message = "Invalidado por no cumplir las políticas del directorio."
            };

        // Encriptar la nueva contraseña.
        var response = await Data.Accounts.Update(account, EncryptClass.Encrypt(newPassword));

        return response;
    }


}