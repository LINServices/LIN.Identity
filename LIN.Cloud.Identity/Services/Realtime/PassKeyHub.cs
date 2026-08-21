namespace LIN.Cloud.Identity.Services.Realtime;

public partial class PassKeyHub(IAccountLogRepository accountLogs) : Hub
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
    public const string AttemptsChannel = "#attempts";


    /// <summary>
    /// Canal de respuestas.
    /// </summary>
    public const string ResponseChannel = "#responses";


    /// <summary>
    /// Evento cuando se desconecta.
    /// </summary>
    public override Task OnDisconnectedAsync(Exception? exception)
    {

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
            Expiration = modelo.Expiration,
            Time = modelo.Time,
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
            JwtModel accountJwt = JwtService.Validate(modelo.Token);

            if (!accountJwt.IsAuthenticated || modelo.Status != PassKeyStatus.Success)
            {
                PassKeyModel badPass = new()
                {
                    Status = modelo.Status,
                    User = modelo.User
                };

                await Clients.Groups($"dbo.{modelo.HubKey}").SendAsync(ResponseChannel, badPass);
                return;
            }

            // Obtiene los intentos.
            var attempt = (from intento in Attempts[modelo.User.ToLower()].Where(A => A.HubKey == modelo.HubKey)
                           where intento.HubKey == modelo.HubKey
                           select intento).FirstOrDefault();

            if (attempt is null)
                return;

            attempt.Status = modelo.Status;

            if (DateTime.UtcNow > modelo.Expiration)
            {
                attempt.Status = PassKeyStatus.Expired;
                attempt.Token = string.Empty;
            }
            else
            {
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

            var responsePasskey = new PassKeyModel()
            {
                Expiration = modelo.Expiration,
                Status = attempt.Status,
                User = attempt.User,
                Token = attempt.Token,
                Time = DateTime.UtcNow,
                HubKey = string.Empty,
                Key = string.Empty
            };

            await accountLogs.Create(new()
            {
                AccountId = accountJwt.AccountId,
                AuthenticationMethod = AuthenticationMethods.Authenticator,
                Time = DateTime.UtcNow,
            });

            await Clients.Groups($"dbo.{modelo.HubKey}").SendAsync(ResponseChannel, responsePasskey);

        }
        catch (Exception)
        {
        }
    }
}