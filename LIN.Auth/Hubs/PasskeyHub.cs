namespace LIN.Auth.Hubs;


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
            Attempts.Add(attempt.User.ToLower(), new() { attempt });
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
    /// 
    /// </summary>
    public async Task ReceiveRequest(PassKeyModel modelo)
    {

        try
        {
            // Obtiene la cuenta
            var cuenta = Attempts[modelo.User.ToLower()];

            // Obtiene el dispositivo
            var intent = cuenta.Where(T => T.HubKey == modelo.HubKey).ToList().FirstOrDefault();

            if (intent == null)
                return;

            intent.Status = modelo.Status;

            if (DateTime.Now > modelo.Expiración)
            {
                intent.Status = PassKeyStatus.Expired;
                modelo.Status = PassKeyStatus.Expired;
                modelo.Token = string.Empty;
                intent.Token = string.Empty;
            }


            var (isValid, _, userID, orgID) = Jwt.Validate(modelo.Token);
            if (isValid && modelo.Status == PassKeyStatus.Success)
            {


                var userInfo = await Data.Accounts.Read(userID, true, true, true);

                var badPass = new PassKeyModel()
                {
                    Status = PassKeyStatus.Failed,
                    User = modelo.User,
                };

                if (userInfo.Response != Responses.Success)
                {
                    await Clients.Groups($"dbo.{modelo.HubKey}").SendAsync("recieveresponse", badPass);
                    return;
                }

                if (userInfo.Model.OrganizationAccess == null || userInfo.Model.OrganizationAccess.Organization.ID == 0)
                {

                }
                else
                {
                    // Validacion de la app
                    var applicationOnOrg = await Data.Applications.AppOnOrg(intent.Application.Key, userInfo.Model.OrganizationAccess.Organization.ID);

                    // Si la app no existe o no esta activa
                    if (applicationOnOrg.Response != Responses.Success)
                    {
                        badPass.Status = PassKeyStatus.BlockedByOrg;
                        await Clients.Groups($"dbo.{modelo.HubKey}").SendAsync("recieveresponse", badPass);
                        return;
                    }

                }

                // Agregar

                _= Data.Logins.Create(new()
                {
                    ID =0,
                    Platform = Platforms.Undefined,
                    AccountID = userInfo.Model.ID,
                    Date = DateTime.Now,
                    Application = new()
                    {
                        Key = intent.Application.Key
                    }
                });


            }

            var pass = new PassKeyModel()
            {
                Expiración = modelo.Expiración,
                Status = modelo.Status,
                User = modelo.User,
                Token = modelo.Token,
                Hora = modelo.Hora,
                Application = new(),
                HubKey = "",
                Key = ""
            };

            await Clients.Groups($"dbo.{modelo.HubKey}").SendAsync("recieveresponse", pass);

        }
        catch (Exception ex)
        {
            await Services.EmailWorker.SendMail("giraldojhong4@gmail.com", "error", $"{ex.Message}\n\n{ex.StackTrace}\n\n{ex.InnerException}");
        }



    }





}
