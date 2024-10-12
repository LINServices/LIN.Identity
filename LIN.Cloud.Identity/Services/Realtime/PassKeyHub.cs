using LIN.Cloud.Identity.Persistence.Models;

namespace LIN.Cloud.Identity.Services.Realtime;

public partial class PassKeyHub(Data.PassKeys passKeysData, Data.AccountLogs accountLogs) : Hub
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

            // Validación del token recibido
            var info = JwtService.Validate(modelo.Token);

            // No es valido el token
            if (!info.IsAuthenticated || modelo.Status != PassKeyStatus.Success)
            {
                // Modelo de falla
                PassKeyModel badPass = new()
                {
                    Status = modelo.Status,
                    User = modelo.User
                };

                // comunica la respuesta
                await Clients.Groups($"dbo.{modelo.HubKey}").SendAsync(ResponseChannel, badPass);
                return;
            }

            // Obtiene el attempt.
            List<PassKeyModel> attempts = Attempts[modelo.User.ToLower()].Where(A => A.HubKey == modelo.HubKey).ToList();

            // Elemento
            var attempt = attempts.Where(A => A.HubKey == modelo.HubKey).FirstOrDefault();

            // Validación del intento
            if (attempt == null)
                return;

            // Eliminar el attempt de la lista
            attempts.Remove(attempt);

            // Cambiar el estado del intento
            attempt.Status = modelo.Status;

            // Si el tiempo de expiración ya paso
            if (DateTime.Now > modelo.Expiración)
            {
                attempt.Status = PassKeyStatus.Expired;
                attempt.Token = string.Empty;
            }

            //// Validación de la organización
            //if (orgID > 0)
            //{
            //    // Obtiene la organización
            //    var organization = await Data.Organizations.Organizations.Read(orgID);

            //    // Si tiene lista blanca
            //    //if (organization.Model.HaveWhiteList)
            //    //{
            //    //    //// Validación de la app
            //    //    //var applicationOnOrg = await Data.Organizations.Applications.AppOnOrg(attempt.Application.Key, orgID);

            //    //    //// Si la app no existe o no esta activa
            //    //    //if (applicationOnOrg.Response != Responses.Success)
            //    //    //{
            //    //    //    // Modelo de falla
            //    //    //    PassKeyModel badPass = new()
            //    //    //    {
            //    //    //        Status = PassKeyStatus.BlockedByOrg,
            //    //    //        User = modelo.User
            //    //    //    };

            //    //    //    // comunica la respuesta
            //    //    //    await Clients.Groups($"dbo.{modelo.HubKey}").SendAsync("#response", badPass);
            //    //    //    return;

            //    //    //}
            //    //}


            //}

            //// Aplicación
            //var app = await Data.Applications.Read(attempt.Application.Key);

            //// Si la app no existe
            //if (app.Response != Responses.Success)
            //{
            //    // Modelo de falla
            //    PassKeyModel badPass = new()
            //    {
            //        Status = PassKeyStatus.Failed,
            //        User = modelo.User
            //    };

            //    // comunica la respuesta
            //    await Clients.Groups($"dbo.{modelo.HubKey}").SendAsync("#response", badPass);
            //    return;
            //}

            //// Guarda el acceso.
            //LoginLogModel loginLog = new()
            //{
            //    AccountID = userID,
            //    Application = new()
            //    {
            //        ID = app.Model.ID
            //    },
            //    Date = DateTime.Now,
            //    Type = LoginTypes.Passkey,
            //    ID = 0
            //};

            //_ = Data.Logins.Create(loginLog);



            _ = passKeysData.Create(new PassKeyDBModel
            {
                AccountId = info.AccountId,
                Id = 0,
                Time = DateTime.Now,
            });

            //// Nuevo token 
            var newToken = JwtService.Generate(new AccountModel()
            {
                Id = info.AccountId,
                Identity = new()
                {
                    Id = info.IdentityId,
                    Unique = info.Unique
                },
                IdentityId = info.IdentityId
            }, 0);

            // nuevo pass
            var pass = new PassKeyModel()
            {
                Expiración = modelo.Expiración,
                Status = modelo.Status,
                User = modelo.User,
                Token = newToken,
                Hora = modelo.Hora,
                HubKey = "",
                Key = ""
            };

            /// <summary>
            await accountLogs.Create(new()
            {
                AccountId = info.AccountId,
                AuthenticationMethod = AuthenticationMethods.Authenticator,
                Time = DateTime.Now,
            });

            // Respuesta al cliente
            await Clients.Groups($"dbo.{modelo.HubKey}").SendAsync(ResponseChannel, pass);

        }
        catch
        {
        }

    }

}