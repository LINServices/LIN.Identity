namespace LIN.Auth.Hubs;


public class AccountHub : Hub
{


    /// <summary>
    /// Lista de cuentas y dispositivos
    /// </summary>
    public readonly static Dictionary<int, List<DeviceModel>> Cuentas = new();



    /// <summary>
    /// Unir un dispositivo a la lista
    /// </summary>
    /// <param name="modelo">Modelo del dispositivo</param>
    public async Task Join(DeviceModel modelo)
    {

        // Validación del token
        var (isValid, _, id) = Jwt.Validate(modelo.Token);

        if (!isValid)
            return;

        // Estados
        modelo.ID = Context.ConnectionId;
        modelo.Estado = DeviceState.Actived;
        modelo.Cuenta = id;

        // Agrega el modelo
        if (!Cuentas.ContainsKey(modelo.Cuenta))
            Cuentas.Add(modelo.Cuenta, new() { modelo });

        else
        {

            var models = Cuentas[modelo.Cuenta];

            var devices = models.Where(x => x.DeviceKey == modelo.DeviceKey).ToList();

            foreach (var device in devices)
                device.Estado = DeviceState.Disconnected;

            Cuentas[modelo.Cuenta].Add(modelo);

        }

        // Grupo de la cuenta
        await Groups.AddToGroupAsync(Context.ConnectionId, modelo.Cuenta.ToString());

        // Envía el nuevo dispositivo
        await RegisterDevice(modelo);

        // Testea la conexión
        await TestDevices(modelo.Cuenta);

    }



    /// <summary>
    /// Evento Salir
    /// </summary>
    /// <param name="cuenta">ID de la cuenta</param>
    public async Task Leave(int cuenta)
    {
        Cuentas[cuenta] = Cuentas[cuenta].Where(T => T.ID != Context.ConnectionId).ToList();
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{cuenta}");
        await Clients.Group(cuenta.ToString()).SendAsync("leaveevent", Context.ConnectionId);
    }



    /// <summary>
    /// Obtener la lista de dispositivos de una cuenta
    /// </summary>
    /// <param name="cuenta">ID de la cuenta</param>
    public async Task GetDevicesList(int cuenta)
    {
        var devices = Cuentas[cuenta] ?? new();
        await Clients.Caller.SendAsync("devicesList", devices.Where(model => model.Estado == DeviceState.Actived), Context.ConnectionId);
    }



    /// <summary>
    /// Envía a los clientes cuando se agrega un nuevo dispositivo
    /// </summary>
    /// <param name="modelo">Modelo del nuevo dispositivo</param>
    public async Task RegisterDevice(DeviceModel modelo)
    {
        await Clients.Group(modelo.Cuenta.ToString()).SendAsync("newdevice", modelo);
    }



    /// <summary>
    /// Testea la conexión de los dispositivos
    /// </summary>
    /// <param name="account">ID de la cuenta</param>
    public async Task TestDevices(int account)
    {
        // Cambia el estado de los dispositivos
        Cuentas[account].ForEach(model => model.Estado = DeviceState.WaitingResponse);
        await Clients.Group(account.ToString()).SendAsync("ontest");
    }




    public async void ReceiveTestStatus(int account, int battery, bool bateryConected)
    {

        try
        {
            // Obtiene la cuenta
            var cuenta = Cuentas[account];

            // Obtiene el dispositivo
            DeviceModel? device = cuenta.Where(T => T.ID == Context.ConnectionId).ToList().FirstOrDefault();

            if (device == null)
                return;

            // Cambia los estados
            device.Estado = DeviceState.Actived;
            device.BateryLevel = battery;
            device.BateryConected = bateryConected;

            // Envia el cambio de dispositivos
            await RegisterDevice(device);
        }
        catch
        {
        }



    }



    public async Task SendDeviceCommand(string receiver, string command)
    {
        await Clients.Client(receiver).SendAsync("devicecommand", command);
    }



    public async Task SendAccountCommand(int receiver, string command)
    {
        await Clients.GroupExcept(receiver.ToString(), new[] { Context.ConnectionId }).SendAsync("accountcommand", command);
    }


  



    public void AddInvitation(List<int> users)
    {
        try
        {

            foreach (var user in users)
            {
                _ = Clients.Group(user.ToString()).SendAsync("invitate", "retrievedata");
            }

        }
        catch
        {

        }

    }


    public override Task OnDisconnectedAsync(Exception? exception)
    {

        var cuenta = Cuentas.Values.Where(T => T.Where(T => T.ID == Context.ConnectionId).Any()).FirstOrDefault();

        if (cuenta == null)
            return base.OnDisconnectedAsync(exception);

        var xx = cuenta.FirstOrDefault()?.Cuenta ?? 0;
        _ = Leave(xx);
        return base.OnDisconnectedAsync(exception);

    }





}
