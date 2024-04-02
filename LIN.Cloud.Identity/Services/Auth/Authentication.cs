namespace LIN.Cloud.Identity.Services.Auth;


public class Authentication
{


    /// <summary>
    /// Usuario.
    /// </summary>
    private string User { get; set; }



    /// <summary>
    /// Usuario.
    /// </summary>
    private string Password { get; set; }



    /// <summary>
    /// Código de la aplicación.
    /// </summary>
    private string AppCode { get; set; }



    /// <summary>
    /// Modelo obtenido.
    /// </summary>
    public AccountModel? Account { get; set; } = null;




    /// <summary>
    /// Generar nuevo servicio de autenticación.
    /// </summary>
    /// <param name="user">Usuario.</param>
    /// <param name="password">Contraseña</param>
    /// <param name="appCode">Código de aplicación</param>
    public Authentication(string user, string password, string appCode)
    {
        this.User = user;
        this.Password = password;
        this.AppCode = appCode;
    }





    /// <summary>
    /// Iniciar el proceso.
    /// </summary>
    public async Task<Responses> Start()
    {

        // Obtener la cuenta.
        var account = await GetAccount();

        // Error.
        if (!account)
            return Responses.NotExistAccount;

        // Validar contraseña.
        bool password = ValidatePassword();

        if(!password)
            return Responses.InvalidPassword;


        return Responses.Success;
    }



    /// <summary>
    /// Iniciar el proceso.
    /// </summary>
    private async Task<bool> GetAccount()
    {

        // Obtener la cuenta.
        var account = await Data.Accounts.Read(User, new()
        {
            FindOn = FindOn.StableAccounts,
            IsAdmin = true
        });

        // Establecer.
        Account = account.Model;

        // Respuesta.
        return account.Response == Responses.Success;

    }



    /// <summary>
    /// Validar la contraseña.
    /// </summary>
    private bool ValidatePassword()
    {

        // Validar la cuenta.
        if (Account == null)
            return false;

        // Validar la contraseña.
        if (Global.Utilities.Cryptography.Encrypt(Password) != Account.Password)
            return false;

        // Correcto.
        return true;

    }


}