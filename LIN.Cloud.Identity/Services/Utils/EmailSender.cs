namespace LIN.Cloud.Identity.Services.Utils;

public class EmailSender(ILogger<EmailSender> logger, IConfigurationManager configuration)
{

    /// <summary>
    /// Enviar correo.
    /// </summary>
    /// <param name="to">A.</param>
    /// <param name="subject">Asunto.</param>
    /// <param name="body">Cuerpo HTML.</param>
    public async Task<bool> Send(string to, string subject, string body)
    {
        try
        {
            // Servicio.
            Global.Http.Services.Client client = new(configuration["hangfire:mail"])
            {
                TimeOut = 10
            };

            client.AddParameter("subject", subject);
            client.AddParameter("mail", to);

            var result = await client.Post(body);

            logger.LogInformation(result);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al enviar correo.");
            return false;
        }
    }

}