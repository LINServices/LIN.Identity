namespace LIN.Cloud.Identity.Services.Realtime;

public partial class PassKeyHub
{

    /// <summary>
    /// Agregar un dispositivo administrador.
    /// </summary>
    public async Task JoinAdmin(string token)
    {
        // Obtener información del token.
        var tokenInformation = JwtService.Validate(token);

        // Validar.
        if (!tokenInformation.IsAuthenticated)
            return;

        // Grupo de la cuenta.
        await Groups.AddToGroupAsync(Context.ConnectionId, BuildGroupName(tokenInformation.Unique));
    }


    /// <summary>
    /// Nuevo intento de inicio.
    /// </summary>
    /// <param name="attempt">Modelo.</param>
    public async Task JoinIntent(PassKeyModel attempt)
    {

        //// Aplicación
        //var application = await Data.Applications.Read(attempt.Application.Key);

        //// Si la app no existe o no esta activa
        //if (application.Response != Responses.Success)
        //    return;

        //// Preparar el modelo
        //attempt.Application ??= new();
        //attempt.Application.Name = application.Model.Name;
        //attempt.Application.Badge = application.Model.Badge;
        //attempt.Application.Key = application.Model.Key;
        //attempt.Application.ID = application.Model.ID;

        // Vencimiento
        var expiración = DateTime.UtcNow.AddMinutes(2);

        // Caducidad el modelo
        attempt.HubKey = Context.ConnectionId;
        attempt.Status = PassKeyStatus.Undefined;
        attempt.Time = DateTime.UtcNow;
        attempt.Expiration = expiración;

        // Agrega el modelo
        if (!Attempts.ContainsKey(attempt.User.ToLower()))
            Attempts.Add(attempt.User.ToLower(), [attempt]);

        else
            Attempts[attempt.User.ToLower()].Add(attempt);

        // Yo
        await Groups.AddToGroupAsync(Context.ConnectionId, $"dbo.{Context.ConnectionId}");

        await SendRequest(attempt);

    }

}