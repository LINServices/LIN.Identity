namespace LIN.Cloud.Identity.Services.Realtime;

public partial class PassKeyHub
{

    /// <summary>
    /// Agregar un dispositivo administrador.
    /// </summary>
    public async Task JoinAdmin(string token)
    {
        var tokenInformation = JwtService.Validate(token);

        if (!tokenInformation.IsAuthenticated)
            return;

        await Groups.AddToGroupAsync(Context.ConnectionId, BuildGroupName(tokenInformation.Unique));
    }


    /// <summary>
    /// Nuevo intento de inicio.
    /// </summary>
    /// <param name="attempt">Modelo.</param>
    public async Task JoinIntent(PassKeyModel attempt)
    {

        var expiración = DateTime.UtcNow.AddMinutes(2);

        attempt.HubKey = Context.ConnectionId;
        attempt.Status = PassKeyStatus.Undefined;
        attempt.Time = DateTime.UtcNow;
        attempt.Expiration = expiración;

        if (!Attempts.ContainsKey(attempt.User.ToLower()))
            Attempts.Add(attempt.User.ToLower(), [attempt]);

        else
            Attempts[attempt.User.ToLower()].Add(attempt);

        // Canal propio de esta conexión, usado para recibir la respuesta directamente.
        await Groups.AddToGroupAsync(Context.ConnectionId, $"dbo.{Context.ConnectionId}");

        await SendRequest(attempt);

    }

}