using Http.Attributes;
using LIN.Cloud.Identity.Persistence.Models;
using FindOn = LIN.Cloud.Identity.Services.Models.FindOn;

namespace LIN.Cloud.Identity.Areas.Authentication;

[Route("[controller]")]
public class SecurityController(Data.Accounts accountsData, Data.OtpService otpService) : AuthenticationBaseController
{

    /// <summary>
    /// Si el usuario olvidó su contraseña, puede solicitar un cambio de contraseña.
    /// </summary>
    /// <param name="user">Usuario.</param>
    [HttpPost("forget/password")]
    [RateLimit(requestLimit: 2, timeWindowSeconds: 60, blockDurationSeconds: 100)]
    public async Task<HttpResponseBase> ForgetPassword([FromQuery] string user)
    {

        // Validar estado del usuario.
        var account = await accountsData.Read(user, new()
        {
            FindOn = FindOn.StableAccounts,
            IncludePhoto = false
        });

        if (account.Response != Responses.Success)
            return new(account.Response)
            {
                Message = "No se puede reestablecer la contraseña de esta cuenta debido a que no existe o esta inactiva."
            };

        // Generar OTP.
        var otpCode = Global.Utilities.KeyGenerator.GenerateOTP(5);

        // Guardar OTP.
        var modelo = new OtpDatabaseModel
        {
            Account = account.Model,
            AccountId = account.Model.Id,
            Code = otpCode,
            ExpireTime = DateTime.Now.AddMinutes(10),
            IsUsed = false
        };

        var created = await otpService.Create(modelo);

        if (created.Response != Responses.Success)
            return new(created.Response)
            {
                Message = "No se pudo crear el código de verificación."
            };

        // Enviar mail.
        var ma = 


    }

}