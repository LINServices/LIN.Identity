namespace LIN.Identity.Areas.Security;


[Route("security")]
public class Security : ControllerBase
{


    /// <summary>
    /// Olvidar contraseña
    /// </summary>
    /// <param name="user">Usuario</param>
    [HttpPost("password/forget")]
    public async Task<HttpReadOneResponse<EmailModel>> ForgetPassword([FromQuery] string user)
    {

        // Nulo o vacío
        if (string.IsNullOrWhiteSpace(user))
            return new(Responses.InvalidParam);


        // Obtiene la conexión
        var (context, contextKey) = Conexión.GetOneConnection();

        // Obtiene la información de usuario
        var userResponse = await Data.Accounts.ReadBasic(user, context);

        // Evalúa la respuesta
        if (userResponse.Response != Responses.Success)
        {
            context.CloseActions(contextKey);
            return new(userResponse.Response);
        }

        // Modelo del usuario
        var userData = userResponse.Model;

        // Obtiene los emails asociados
        var emailResponse = await Data.Mails.ReadVerifiedEmails(userData.ID, context);

        // Evalúa la respuesta
        if (emailResponse.Response != Responses.Success)
        {
            context.CloseActions(contextKey);
            return new(emailResponse.Response);
        }

        // Modelos de emails
        var emailsData = emailResponse.Models;

        // Emails verificados
        var verifiedMail = emailsData.FirstOrDefault(mail => mail.IsDefault);

        // Valida
        if (verifiedMail == null || !Mail.Validar(verifiedMail.Email ?? string.Empty))
        {
            context.CloseActions(contextKey);
            return new();
        }

        // Modelo del link
        var link = new UniqueLink()
        {
            Key = KeyGen.Generate(30, string.Empty),
            AccountID = userData.ID,
            Status = MagicLinkStatus.Activated,
            Vencimiento = DateTime.Now.AddMinutes(30)
        };

        // Crea el nuevo link
        var linkResponse = await Data.Links.Create(link);

        // Evalúa
        if (linkResponse.Response != Responses.Success)
        {
            context.CloseActions(contextKey);
            return new(Responses.NotRows);
        }


        // Envía el correo
        await EmailWorker.SendPassword(verifiedMail?.Email!, userData.Usuario, $"http://linapps.co/resetpassword/{userData.ID}/{link.Key}");


        return new(Responses.Success, new()
        {
            Email = CensorEmail(verifiedMail?.Email ?? "")
        });
    }



    private static string CensorEmail(string email)
    {
        var atIndex = email.IndexOf('@');

        if (atIndex >= 0)
        {
            var dotIndex = email.IndexOf('.');
            if (dotIndex > atIndex + 3)
            {
                var username = email.Substring(0, atIndex - 3); // Censura los últimos 3 caracteres antes del "@".
                var domain = email.Substring(atIndex); // Censura el dominio antes del primer punto.
                return $"{username}***{domain}";
            }
        }
        return email;
    }







    /// <summary>
    /// Reestablece la contraseña
    /// </summary>
    /// <param name="key">Llave del link</param>
    /// <param name="modelo">Nuevo modelo</param>
    [HttpPatch("password/reset")]
    public async Task<HttpResponseBase> ResetPassword([FromHeader] string key, [FromBody] UpdatePasswordModel modelo)
    {

        // Nulo o vacío
        if (string.IsNullOrWhiteSpace(key))
            return new(Responses.InvalidParam);

        // Link
        var link = await Data.Links.ReadOneAnChange(key);

        // Evalúa
        if (link.Response != Responses.Success)
        {
            return new(Responses.Unauthorized);
        }

        // Establece el id
        modelo.Account = link.Model.AccountID;

        // Respuesta
        var updateResponse = await Data.Accounts.Update(modelo);

        if (updateResponse.Response != Responses.Success)
            return new();

        return new(Responses.Success);
    }



    /// <summary>
    /// Verifica un correo
    /// </summary>
    /// <param name="key">Key de acceso LINK</param>
    [HttpPost("mails/verify")]
    public async Task<ResponseBase> VerifyEmail([FromHeader] string key)
    {

        // Obtiene una conexión
        var (context, contextKey) = Conexión.GetOneConnection();

        // Obtiene un link
        var linkResponse = await Data.MailLinks.ReadAndDisable(key);

        // Evalúa
        if (linkResponse.Response != Responses.Success)
        {
            context.CloseActions(contextKey);
            return new();
        }

        // Modelo del link
        var linkData = linkResponse.Model;


        // Obtiene el email
        var emailResponse = await Data.Mails.Read(linkData.Email, context);

        // Evalúa
        if (emailResponse.Response != Responses.Success)
        {
            context.CloseActions(contextKey);
            return new();
        }

        // Modelo del mail
        var emailData = emailResponse.Model;

        // Actualiza el estado del mail
        var updateState = await Data.Mails.UpdateState(emailData.ID, EmailStatus.Verified, context);

        // Evalúa
        if (updateState.Response != Responses.Success)
        {
            context.CloseActions(contextKey);
            return new();
        }


        // Comprobación de email default
        {

            // Respuesta
            var emails = await Data.Mails.ReadVerifiedEmails(emailData.UserID);

            // Evalúa
            if (emails.Response == Responses.Success)
            {
                // Existe el mail default
                var haveDefault = emails.Models.Where(E => E.IsDefault).Any();

                // Si no existe
                if (!haveDefault)
                {
                    // Establece el mail actual
                    var res = await Data.Mails.SetDefaultEmail(emailData.UserID, emailData.ID);

                }

            }

        }

        return new(Responses.Success);
    }



    /// <summary>
    /// Reenvía el correo para la activación
    /// </summary>
    /// <param name="password">Contraseña de la cuenta</param>
    /// <param name="model">Modelo del email</param>
    [HttpPost("mails/resend")]
    public async Task<ResponseBase> EmailResend([FromHeader] int mailID, [FromHeader] string token)
    {


        var (isValid, _, userID, _, _) = Jwt.Validate(token);

        if (!isValid)
        {
            return new(Responses.Unauthorized);
        }

        var mailResponse = await Data.Mails.Read(mailID);

        // Evaluación de la respuesta
        if (mailResponse.Response != Responses.Success)
        {
            return new(Responses.NotRows);
        }

        var mailData = mailResponse.Model;

        if (mailData.Status == EmailStatus.Verified)
        {
            return new(Responses.Undefined);
        }




        // Crea el LINK
        var emailLink = new MailMagicLink()
        {
            Email = mailData.ID,
            Status = MagicLinkStatus.Activated,
            Vencimiento = DateTime.Now.AddMinutes(10),
            Key = KeyGen.Generate(20, "ml")
        };


        try
        {

            var add = await Data.MailLinks.Create(emailLink);


            if (add.Response != Responses.Success)
            {
                return new();
            }

            var user = (await Data.Accounts.ReadBasic(userID)).Model;

            var bytes = Encoding.UTF8.GetBytes(mailData.Email);
            var mail64 = Convert.ToBase64String(bytes);

            bytes = Encoding.UTF8.GetBytes(user.Usuario);
            var user64 = Convert.ToBase64String(bytes);

            await EmailWorker.SendVerification(mailData.Email, $"http://linaccount.somee.com/verificate/{user64}/{mail64}/{emailLink.Key}", mailData.Email);
            return new(Responses.Success);

        }
        catch
        {

        }
        finally
        {
        }

        return new();

    }



}