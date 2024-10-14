namespace LIN.Cloud.Identity.Services.Realtime;

public partial class PassKeyHub(Data.AccountLogs accountLogs) : Hub
{

    /// <summary>
    /// Lista de intentos Passkey.
    /// String: Usuario.
    /// PasskeyModels: Lista de intentos. 
    /// </summary>
    public static readonly Dictionary<string, List<PassKeyModel>> Attempts = [];


    /// <summary>
    /// Canal de intentos.
    /// </summary>
    public static readonly string AttemptsChannel = "#attempts";


    /// <summary>
    /// Canal de respuestas.
    /// </summary>
    public static readonly string ResponseChannel = "#responses";


    /// <summary>
    /// Evento cuando se desconecta.
    /// </summary>

    public override Task OnDisconnectedAsync(Exception? exception)
    {

        // Obtener el intento.
        var attempt = Attempts.Values.Where(T => T.Where(T => T.HubKey == Context.ConnectionId).Any()).FirstOrDefault() ?? new();

        _ = attempt.Where(T =>
        {
            if (T.HubKey == Context.ConnectionId && T.Status == PassKeyStatus.Undefined)
                T.Status = PassKeyStatus.Failed;

            return false;
        });

        return base.OnDisconnectedAsync(exception);
    }


    //=========== Dispositivos ===========//


    /// <summary>
    /// Envía la solicitud a los admins.
    /// </summary>
    public async Task SendRequest(PassKeyModel modelo)
    {

        var pass = new PassKeyModel()
        {
            Expiración = modelo.Expiración,
            Hora = modelo.Hora,
            Status = modelo.Status,
            User = modelo.User,
            HubKey = modelo.HubKey
        };

        await Clients.Group(BuildGroupName(modelo.User)).SendAsync(AttemptsChannel, pass);
    }


    /// <summary>
    /// Recibe una respuesta de passkey
    /// </summary>
    public async Task ReceiveRequest(PassKeyModel modelo)
    {
        try
        {

            // Obtener información del token.
            JwtModel accountJwt = JwtService.Validate(modelo.Token);

            // Validar el token.
            if (!accountJwt.IsAuthenticated || modelo.Status != PassKeyStatus.Success)
            {
                // Modelo de falla
                PassKeyModel badPass = new()
                {
                    Status = modelo.Status,
                    User = modelo.User
                };

                // Enviar respuesta.
                await Clients.Groups($"dbo.{modelo.HubKey}").SendAsync(ResponseChannel, badPass);
                return;
            }

            // Obtiene los intentos.
            var attempt = (from intento in Attempts[modelo.User.ToLower()].Where(A => A.HubKey == modelo.HubKey)
                           where intento.HubKey == modelo.HubKey
                           select intento).FirstOrDefault();

            // No se encontró.
            if (attempt is null)
                return;

            // Cambiar el estado del intento.
            attempt.Status = modelo.Status;

            // Si el tiempo de expiración ya paso
            if (DateTime.Now > modelo.Expiración)
            {
                attempt.Status = PassKeyStatus.Expired;
                attempt.Token = string.Empty;
            }
            else
            {
                // Generar nuevo token.
                string token = JwtService.Generate(new()
                {
                    Id = accountJwt.AccountId,
                    IdentityId = accountJwt.IdentityId,
                    Identity = new()
                    {
                        Id = accountJwt.IdentityId,
                        Unique = accountJwt.Unique
                    },
                }, 0);

                attempt.Token = token;
            }

            // Respuesta passkey.
            var responsePasskey = new PassKeyModel()
            {
                Expiración = modelo.Expiración,
                Status = attempt.Status,
                User = attempt.User,
                Token = attempt.Token,
                Hora = DateTime.Now,
                HubKey = string.Empty,
                Key = string.Empty
            };

            // Crear log.
            await accountLogs.Create(new()
            {
                AccountId = accountJwt.AccountId,
                AuthenticationMethod = AuthenticationMethods.Authenticator,
                Time = DateTime.Now,
            });

            // Respuesta al cliente.
            await Clients.Groups($"dbo.{modelo.HubKey}").SendAsync(ResponseChannel, responsePasskey);

        }
        catch (Exception)
        {
        }
    }
}