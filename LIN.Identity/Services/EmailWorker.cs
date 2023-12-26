﻿namespace LIN.Identity.Services;


public class EmailWorker
{


    /// <summary>
    /// Email de salida
    /// </summary>
    private static string Password { get; set; } = string.Empty;



    /// <summary>
    /// Inicia el servicio
    /// </summary>
    public static void StarService()
    {
        Password = Configuration.GetConfiguration("resend:key");
    }



    /// <summary>
    /// Enviar un correo
    /// </summary>
    /// <param name="to">Destinatario</param>
    /// <param name="url">url</param>
    /// <param name="mail">Info del mail</param>
    public static async Task<bool> SendVerification(string to, string url, string mail)
    {

        // Obtiene la plantilla
        var body = File.ReadAllText("wwwroot/Plantillas/Plantilla.html");

        // Remplaza
        body = body.Replace("@@Titulo", "Verificación de correo electrónico");
        body = body.Replace("@@Subtitulo", $"{mail}");
        body = body.Replace("@@Url", url);
        body = body.Replace("@@Mensaje", "Hemos recibido tu solicitud para agregar una dirección de correo electrónico adicional a tu cuenta. Para completar este proceso, da click en el siguiente botón");
        body = body.Replace("@@ButtonMessage", "Verificar");

        // Envía el email
        return await SendMail(to, "Verifica el email", body);

    }



    /// <summary>
    /// Enviar un correo
    /// </summary>
    /// <param name="to">Destinatario</param>
    /// <param name="nombre">Nombre del usuario</param>
    /// <param name="url">URL</param>
    public static async Task<bool> SendPassword(string to, string nombre, string url)
    {

        // Obtiene la plantilla
        var body = File.ReadAllText("wwwroot/Plantillas/Plantilla.html");

        // Remplaza
        body = body.Replace("@@Titulo", "Reestablecer contraseña");
        body = body.Replace("@@Subtitulo", $"Hola, {nombre}");
        body = body.Replace("@@Url", url);
        body = body.Replace("@@Mensaje", "Recibimos tu solicitud para restablecer la contraseña de tu cuenta LIN. Para completar este proceso, simplemente haz clic en el siguiente botón");
        body = body.Replace("@@ButtonMessage", "Cambiar contraseña");

        // Envía el email
        return await SendMail(to, "Cambiar contraseña", body);

    }



    /// <summary>
    /// Enviar un correo
    /// </summary>
    /// <param name="to">Destinatario</param>
    /// <param name="asunto">Asunto</param>
    /// <param name="body">Cuerpo del correo</param>
    public static async Task<bool> SendMail(string to, string asunto, string body)
    {
        try
        {

            using var client = new HttpClient();
            var url = "https://api.resend.com/emails";
            var accessToken = Password; // Reemplaza con tu token de acceso

            var requestData = new
            {
                from = "onboarding@resend.dev",
                to = new[]
                {
                  to
                },
                subject = asunto
            };


            var json = System.Text.Json.JsonSerializer.Serialize(requestData);

            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                _ = Logger.Log("Error al enviar un correo: ", response.StatusCode.ToString(), 3);
            }
            return true;
        }
        catch (Exception ex)
        {
            _ = Logger.Log(ex, 2);
        }
        return false;
    }



}