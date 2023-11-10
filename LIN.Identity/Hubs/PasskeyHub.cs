namespace LIN.Identity.Hubs;


public class PassKeyHub : Hub
{


    /// <summary>
    /// Lista de intentos Passkey
    /// </summary>
    public readonly static Dictionary<string, List<PassKeyModel>> Attempts = new();



    /// <summary>
    /// Nuevo intento passkey
    /// </summary>
    /// <param name="attempt">Intento passkey</param>
    public async Task JoinIntent(PassKeyModel attempt)
    {

        // Aplicación
        var application = await Data.Applications.Read(attempt.Application.Key);

        // Si la app no existe o no esta activa
        if (application.Response != Responses.Success)
            return;

        // Preparar el modelo
        attempt.Application ??= new();
        attempt.Application.Name = application.Model.Name;
        attempt.Application.Badge = application.Model.Badge;
        attempt.Application.Key = application.Model.Key;
        attempt.Application.ID = application.Model.ID;

        // Vencimiento
        var expiración = DateTime.Now.AddMinutes(2);

        // Caducidad el modelo
        attempt.HubKey = Context.ConnectionId;
        attempt.Status = PassKeyStatus.Undefined;
        attempt.Hora = DateTime.Now;
        attempt.Expiración = expiración;

        // Agrega el modelo
        if (!Attempts.ContainsKey(attempt.User.ToLower()))
            Attempts.Add(attempt.User.ToLower(), new()
            {
                attempt
            });
        else
            Attempts[attempt.User.ToLower()].Add(attempt);

        // Yo
        await Groups.AddToGroupAsync(Context.ConnectionId, $"dbo.{Context.ConnectionId}");

        await SendRequest(attempt);

    }
























    public override Task OnDisconnectedAsync(Exception? exception)
    {

        var e = Attempts.Values.Where(T => T.Where(T => T.HubKey == Context.ConnectionId).Any()).FirstOrDefault() ?? new();


        _ = e.Where(T =>
        {
            if (T.HubKey == Context.ConnectionId && T.Status == PassKeyStatus.Undefined)
                T.Status = PassKeyStatus.Failed;

            return false;
        });


        return base.OnDisconnectedAsync(exception);
    }


    /// <summary>
    /// Un dispositivo envia el PassKey intent
    /// </summary>
    public async Task JoinAdmin(string usuario)
    {

        // Grupo de la cuenta
        await Groups.AddToGroupAsync(Context.ConnectionId, usuario.ToLower());

    }







    //=========== Dispositivos ===========//


    /// <summary>
    /// Envía la solicitud a los admins
    /// </summary>
    public async Task SendRequest(PassKeyModel modelo)
    {

        var pass = new PassKeyModel()
        {
            Expiración = modelo.Expiración,
            Hora = modelo.Hora,
            Status = modelo.Status,
            User = modelo.User,
            HubKey = modelo.HubKey,
            Application = new()
            {
                Name = modelo.Application.Name,
                Badge = modelo.Application.Badge
            }
        };

        await Clients.Group(modelo.User.ToLower()).SendAsync("newintent", pass);
    }




    /// <summary>
    /// Recibe una respuesta de passkey
    /// </summary>
    public async Task ReceiveRequest(PassKeyModel modelo)
    {
        try
        {

            // Validación del token recibido
            var (isValid, userUnique, userID, orgID, _) = Jwt.Validate(modelo.Token);

            // No es valido el token
            if (!isValid || modelo.Status != PassKeyStatus.Success)
            {
                // Modelo de falla
                PassKeyModel badPass = new()
                {
                    Status = modelo.Status,
                    User = modelo.User
                };

                // comunica la respuesta
                await Clients.Groups($"dbo.{modelo.HubKey}").SendAsync("#response", badPass);
                return;
            }

            // Obtiene el attempt
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

            // Validación de la organización
            if (orgID > 0)
            {
                // Obtiene la organización
                var organization = await Data.Organizations.Organizations.Read(orgID);

                // Si tiene lista blanca
                if (organization.Model.HaveWhiteList)
                {
                    // Validación de la app
                    var applicationOnOrg = await Data.Organizations.Applications.AppOnOrg(attempt.Application.Key, orgID);

                    // Si la app no existe o no esta activa
                    if (applicationOnOrg.Response != Responses.Success)
                    {
                        // Modelo de falla
                        PassKeyModel badPass = new()
                        {
                            Status = PassKeyStatus.BlockedByOrg,
                            User = modelo.User
                        };

                        // comunica la respuesta
                        await Clients.Groups($"dbo.{modelo.HubKey}").SendAsync("#response", badPass);
                        return;

                    }
                }


            }

            // Aplicación
            var app = await Data.Applications.Read(attempt.Application.Key);

            // Si la app no existe
            if (app.Response != Responses.Success)
            {
                // Modelo de falla
                PassKeyModel badPass = new()
                {
                    Status = PassKeyStatus.Failed,
                    User = modelo.User
                };

                // comunica la respuesta
                await Clients.Groups($"dbo.{modelo.HubKey}").SendAsync("#response", badPass);
                return;
            }

            // Guarda el acceso.
            LoginLogModel loginLog = new()
            {
                AccountID = userID,
                Application = new()
                {
                    ID = app.Model.ID
                },
                Date = DateTime.Now,
                Platform = Platforms.Undefined,
                Type = LoginTypes.Passkey,
                ID = 0
            };

            _ = Data.Logins.Create(loginLog);


            // Nuevo token 
            var newToken = Jwt.Generate(new()
            {
                ID = userID,
                Usuario = userUnique,
                OrganizationAccess = new()
                {
                    Organization = new()
                    {
                        ID = orgID
                    }
                }
            }, app.Model.ID);

            // nuevo pass
            var pass = new PassKeyModel()
            {
                Expiración = modelo.Expiración,
                Status = modelo.Status,
                User = modelo.User,
                Token = newToken,
                Hora = modelo.Hora,
                Application = new(),
                HubKey = "",
                Key = ""
            };

            // Respuesta al cliente
            await Clients.Groups($"dbo.{modelo.HubKey}").SendAsync("#response", pass);

        }
        catch
        {
        }

    }





}