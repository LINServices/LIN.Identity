namespace LIN.Cloud.Identity.Areas.Authentication;

[Route("[controller]")]
public class SecurityController(Data.Accounts accountsData, Data.OtpService otpService, EmailSender emailSender, Data.Mails mails) : AuthenticationBaseController
{

    /// <summary>
    /// Agregar un correo a una cuenta.
    /// </summary>
    /// <param name="email">Correo.</param>
    [HttpPost("mail")]
    [IdentityToken]
    public async Task<HttpResponseBase> AddMail([FromQuery] string email)
    {
        // Generar modelo del correo.
        var model = new MailModel()
        {
            Mail = email,
            AccountId = UserInformation.AccountId,
            IsPrincipal = false,
            IsVerified = false
        };

        // Respuesta.
        var responseCreate = await mails.Create(model);

        // Si hubo un error.
        switch (responseCreate.Response)
        {
            // Correcto.
            case Responses.Success:
                break;

            // Ya estaba registrado.
            case Responses.ResourceExist:
                return new(responseCreate.Response)
                {
                    Message = $"Hubo un error al agregar el correo <{email}> a la cuenta {model.Account.Identity.Unique}",
                    Errors = [
                        new() {
                            Tittle = "Mail duplicado",
                            Description = "El correo ya se encuentra registrado en el sistema."
                        }
                    ]
                };
            default:
                return new(responseCreate.Response)
                {
                    Message = $"Hubo un error al agregar el correo <{email}> a la cuenta {model.Account.Identity.Unique}"
                };
        }

        // Generar Otp.
        var otpCode = Global.Utilities.KeyGenerator.GenerateOTP(5);

        // Guardar OTP.
        var otpCreateResponse = await otpService.Create(new MailOtpDatabaseModel
        {
            MailModel = responseCreate.Model,
            OtpDatabaseModel = new()
            {
                Account = new() { Id = UserInformation.AccountId },
                Code = otpCode,
                ExpireTime = DateTime.Now.AddMinutes(10),
                IsUsed = false
            }
        });

        // Enviar correo de verificación.
        if (otpCreateResponse.Response != Responses.Success)
            return new()
            {
                Message = "Hubo un error al guardar el código OTP."
            };

        // Enviar correo.
        var success = await emailSender.Send(email, "Verificar", $"Verificar tu correo {otpCode}");

        return new(success ? Responses.Success : Responses.UnavailableService);

    }


    /// <summary>
    /// Validar un correo.
    /// </summary>
    /// <param name="mail">Correo a validar.</param>
    /// <param name="code">Código OTP.</param>
    [HttpPost("validate")]
    public async Task<HttpResponseBase> Validate([FromQuery] string mail, [FromQuery] string code)
    {
        // Validar OTP. 
        var response = await mails.ValidateOtpFormail(mail, code);
        return response;
    }


    /// <summary>
    /// Si un usuario olvido la contraseña.
    /// </summary>
    /// <param name="user">Usuario que olvido.</param>
    [HttpPost("forget/password")]
    public async Task<HttpResponseBase> ForgetPassword([FromQuery] string user)
    {

        // Validar estado del usuario.
        var account = await accountsData.Read(user, new()
        {
            FindOn = FindOn.StableAccounts,
            IncludePhoto = false
        });

        if (account.Response != Responses.Success)
            return new(Responses.NotExistAccount)
            {
                Message = "No se puede reestablecer la contraseña de esta cuenta debido a que no existe o esta inactiva."
            };

        // Obtener mail principal.
        var mail = await mails.ReadPrincipal(user);

        if (mail.Response != Responses.Success)
            return new(Responses.NotRows)
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

        // Crear OTP.
        var created = await otpService.Create(modelo);

        // Si hubo un error.
        if (created.Response != Responses.Success)
            return new(created.Response)
            {
                Message = "No se pudo crear el código de verificación."
            };

        // Enviar mail.
        var success = await emailSender.Send(mail.Model.Mail, "Recuperación de contraseña", $"Su código de verificación es: {otpCode}");

        return new(success ? Responses.Success : Responses.UnavailableService);

    }


    /// <summary>
    /// Reestablecer una contraseña.
    /// </summary>
    /// <param name="code">Código OTP.</param>
    /// <param name="unique">Usuario único.</param>
    /// <param name="newPassword">Nueva contraseña.</param>
    [HttpPost("reset")]
    public async Task<HttpResponseBase> Reset([FromQuery] string code, [FromQuery] string unique, [FromQuery] string newPassword)
    {

        // Validar nueva contraseña.
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

        // Si hubo un error al obtener la cuenta.
        if (account.Response != Responses.Success)
            return new(Responses.NotExistAccount)
            {
                Message = "No se puede reestablecer la contraseña de esta cuenta debido a que no existe o esta inactiva."
            };

        // Leer y actualizar OTP.
        var response = await otpService.ReadAndUpdate(account.Model.Id, code);

        // Si hubo un error al leer y actualizar el código.
        if (response.Response != Responses.Success)
            return new(Responses.Unauthorized)
            {
                Message = "No se pudo reestablecer la contraseña.",
                Errors = [
                    new()
                    {
                        Tittle = "Código OTP invalido",
                        Description = "El código es invalido o ya venció."
                    }
                ]
            };

        // Encriptar contraseña.
        newPassword = Global.Utilities.Cryptography.Encrypt(newPassword);

        // Actualizar contraseña.
        var update = await accountsData.UpdatePassword(account.Model.Id, newPassword);

        return update;


    }

}