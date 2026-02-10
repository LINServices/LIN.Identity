using System.Text;

namespace LIN.Cloud.Identity.Services.Utils;

public class EmailService(IConfiguration configuration) : IEmailService
{

    private string? _requestUri = null;
    private string? _key = null;
    private string? _from = null;

    /// <summary>
    /// Iniciar servicio.
    /// </summary>
    private void Service()
    {
        _requestUri ??= configuration["mailing:url"] ?? string.Empty;
        _key ??= configuration["mailing:key"] ?? string.Empty;
        _from ??= configuration["mailing:from"] ?? string.Empty;
    }


    /// <summary>
    /// Enviar correo.
    /// </summary>
    /// <param name="to">Para.</param>
    /// <param name="person">Persona.</param>
    /// <param name="subject">Asunto.</param>
    /// <param name="body">Cuerpo del mensaje.</param>
    public async Task<bool> SendMail(string[] to, string person, string subject, string body)
    {
        try
        {
            // Iniciar.
            Service();

            // Contenido.
            var jsonData = new
            {
                from = $"{person} <{_from}>",
                to,
                subject,
                html = body
            };

            // Serializar.
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(jsonData);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            HttpClient client = new()
            {
                Timeout = TimeSpan.FromSeconds(10)
            };

            // Authorization.
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_key}");

            // Enviar.
            HttpResponseMessage response = await client.PostAsync(_requestUri, content);

            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
        }
        return false;
    }

}