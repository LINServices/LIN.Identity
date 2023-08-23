namespace LIN.Auth.Hubs;


public class PassKeyHub : Hub
{


    /// <summary>
    /// Lista de intentos Passkey
    /// </summary>
    public readonly static Dictionary<string, List<PassKeyModel>> Attempts = new();




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
    public async Task JoinIntent(PassKeyModel modelo)
    {

        var expiracion = DateTime.Now.AddMinutes(2);

        // Modelo
        modelo.HubKey = Context.ConnectionId;
        modelo.Status = PassKeyStatus.Undefined;
        modelo.Hora = DateTime.Now;
        modelo.Expiración = expiracion;

        // Agrega el modelo
        if (!Attempts.ContainsKey(modelo.User.ToLower()))
            Attempts.Add(modelo.User.ToLower(), new() { modelo });
        else
            Attempts[modelo.User.ToLower()].Add(modelo);

        // Yo
        await Groups.AddToGroupAsync(Context.ConnectionId, $"dbo.{Context.ConnectionId}");

        await SendRequest(modelo);

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
    /// Envia la solicitud a los admins
    /// </summary>
    public async Task SendRequest(PassKeyModel modelo)
    {

        var app = await Data.Applications.Read(modelo.ApplicationKey);


        if (app.Response != Responses.Success)
        {
            return;
        }

        modelo.Application.Name = app.Model.Name;
        modelo.Application.Badge = app.Model.Badge;
        modelo.ApplicationKey = "<Secret>";

        await Clients.Group(modelo.User.ToLower()).SendAsync("newintent", modelo);

    }




    /// <summary>
    /// 
    /// </summary>
    public async void ReceiveRequest(PassKeyModel modelo)
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

            await Clients.Groups($"dbo.{modelo.HubKey}").SendAsync("recieveresponse", modelo);

        }
        catch (Exception ex)
        {
        }



    }





}
