namespace LIN.Cloud.Identity.Services.Utils;

public class EmailSender
{

    /// <summary>
    /// Enviar correo.
    /// </summary>
    /// <param name="to">A.</param>
    /// <param name="subject">Asunto.</param>
    /// <param name="body">Cuerpo HTML.</param>
    public async Task<bool> Send(string to, string subject, string body)
    {

        // Servicio.
        Global.Http.Services.Client client = new(Http.Services.Configuration.GetConfiguration("hangfire:mail"))
        {
            TimeOut = 10
        };

        client.AddParameter("subject", subject);
        client.AddParameter("mail", to);

        var aa = await client.Post(body);

        return true;
    }

}