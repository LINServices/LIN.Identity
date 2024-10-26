using Http.Attributes;
using LIN.Cloud.Identity.Persistence.Models;
using LIN.Cloud.Identity.Services.Utils;
using FindOn = LIN.Cloud.Identity.Services.Models.FindOn;

namespace LIN.Cloud.Identity.Areas.Authentication;

[Route("[controller]")]
public class SecurityController(Data.Accounts accountsData, Data.OtpService otpService, EmailSender emailSender, Data.Mails mails) : AuthenticationBaseController
{


    /// <summary>
    /// Si el usuario olvidó su contraseña, puede solicitar un cambio de contraseña.
    /// </summary>
    /// <param name="user">Usuario.</param>
    [HttpPost("mail")]
    [IdentityToken]
    public async Task<HttpResponseBase> AddMail([FromQuery] string email)
    {

        var model = new MailModel()
        {
            Mail = email,
            AccountId = UserInformation.AccountId,
            IsPrincipal = false,
            IsVerified = false
        };

        var responseCreate = await mails.Create(model);

        if (responseCreate.Response != Responses.Success)
            return new(responseCreate.Response)
            {
                Message = "No se pudo agregar el correo."
            };

        // Generar Otp.
        var x = Global.Utilities.KeyGenerator.GenerateOTP(5);

        //Guardar OTP.
        var xxx = await otpService.Create(new MailOtpDatabaseModel
        {
            MailModel = responseCreate.Model,
            OtpDatabaseModel = new()
            {
                Account = new() { Id = UserInformation.AccountId },
                Code = x,
                ExpireTime = DateTime.Now.AddMinutes(10),
                IsUsed = false
            }
        });

        // Enviar correo de verificación.
        if (xxx.Response != Responses.Success)
            return new();


        // Coreeo
        var ma = await emailSender.Send(email, "Verficar correo", $"Verificar tu correo {x}");

        return new(ma ? Responses.Success : Responses.UnavailableService);

    }


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

        // Obtener mail principal.
        var mail = await mails.ReadPrincipal(user);

        if (mail.Response != Responses.Success)
            return new(mail.Response)
            {
                Message = "Esta cuenta no tiene un correo principal establecido."
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
        var ma = await emailSender.Send(mail.Model.Mail, "Recuperación de contraseña", $"Su código de verificación es: {otpCode}");

        return new(ma ? Responses.Success : Responses.UnavailableService);

    }



    [HttpPost("validate")]
    public async Task<HttpResponseBase> Validate([FromQuery] string mail, [FromQuery] string code)
    {


        var response = await mails.ValidateOtpFormail(mail, code);

        return response;


    }



    [HttpPost("reset")]
    public async Task<HttpResponseBase> Reset([FromQuery] string code, [FromQuery] string unique, [FromQuery] string newPassword)
    {


        if (newPassword is null || newPassword.Length <= 0)
            return new(Responses.InvalidParam)
            {
                Errors = []
            };

        // Validar OTP.
        var account = await accountsData.Read(unique, new()
        {
            FindOn = FindOn.StableAccounts,
            IncludePhoto = false
        });

        if (account.Response != Responses.Success)
            return new(account.Response)
            {
                Message = "No se puede reestablecer la contraseña de esta cuenta debido a que no existe o esta inactiva."
            };


        var response = await otpService.ReadAndUpdate(account.Model.Id, code);

        if (response.Response != Responses.Success)
            return new(response.Response)
            {
                Message = "No se pudo reestablecer la contraseña."
            };


        
        // Encriptar contraseña.
        newPassword = Global.Utilities.Cryptography.Encrypt(newPassword);


        var up = await accountsData.UpdatePassword(account.Model.Id, newPassword);

        return up;


    }






}