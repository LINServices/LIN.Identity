namespace LIN.Identity.Controllers;


[Route("mails")]
public class MailController : ControllerBase
{


    /// <summary>
    /// Obtiene los mails asociados a una cuenta.
    /// </summary>
    /// <param name="token">Token de acceso</param>
    [HttpGet("all")]
    public async Task<HttpReadAllResponse<EmailModel>> GetMails([FromHeader] string token)
    {

        // Información del token.
        var (isValid, _, id, _, _) = Jwt.Validate(token);

        // Validación del token
        if (!isValid)
            return new()
            {
                Response = Responses.Unauthorized,
                Message = "Token invalido."
            };

        return await Data.Mails.ReadAll(id);

    }



    /// <summary>
    /// Agrega un nuevo email a una cuenta.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    /// <param name="password">Contraseña de la cuenta.</param>
    /// <param name="model">Modelo del email.</param>
    [HttpPost("add")]
    public async Task<ResponseBase> EmailAdd([FromHeader] string token, [FromHeader] string password, [FromBody] EmailModel model)
    {

        // Obtener el usuario
        var userData = await Data.Accounts.ReadBasic(model.UserID);

        // Evaluación de la respuesta
        if (userData.Response != Responses.Success)
        {
            return new(Responses.NotExistAccount);
        }

        // Evaluación de la contraseña
        if (userData.Model.Contraseña != EncryptClass.Encrypt(password))
            return new(Responses.Unauthorized);

        // Conexión
        var (context, connectionKey) = Conexión.GetOneConnection();

        // Obtiene los mails actuales
        var mails = await Data.Mails.ReadAll(userData.Model.ID, context);

        // Evalúa la respuesta
        if (mails.Response != Responses.Success)
        {
            context.CloseActions(connectionKey);
            return new();
        }

        // Este email ya esta en la cuenta?
        var countEmail = mails.Models.Where(T => T.Email == model.Email).Count();

        // Si ya existía
        if (countEmail > 0)
        {
            context.CloseActions(connectionKey);
            return new();
        }

        // Agrega el mail
        var addMail = await Data.Mails.Create(new()
        {
            Status = EmailStatus.Unverified,
            Email = model.Email,
            ID = 0,
            UserID = userData.Model.ID
        }, context);


        // Evalúa la respuesta
        if (addMail.Response != Responses.Success)
        {
            context.CloseActions(connectionKey);
            return new();
        }

        //Crea el LINK
        var emailLink = new MailMagicLink()
        {
            Email = addMail.LastID,
            Status = MagicLinkStatus.Activated,
            Vencimiento = DateTime.Now.AddMinutes(10),
            Key = KeyGen.Generate(20, "eml")
        };

        try
        {
            // Agrega y guarda el link
            context.DataBase.MailMagicLinks.Add(emailLink);


            var bytes = Encoding.UTF8.GetBytes(model.Email);
            var mail64 = Convert.ToBase64String(bytes);

            bytes = Encoding.UTF8.GetBytes(userData.Model.Usuario);
            var user64 = Convert.ToBase64String(bytes);

            await EmailWorker.SendVerification(model.Email, $"http://linaccount.somee.com/verificate/{user64}/{mail64}/{emailLink.Key}", model.Email);
            return new(Responses.Success);

        }
        catch
        {
        }
        finally
        {
            context.DataBase.SaveChanges();
        }

        return new();

    }


}