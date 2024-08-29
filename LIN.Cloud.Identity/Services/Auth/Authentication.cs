namespace LIN.Cloud.Identity.Services.Auth;


public class Authentication(Data.Accounts accountData) : Interfaces.IAuthentication
{


    /// <summary>
    /// Usuario.
    /// </summary>
    private string User { get; set; } = string.Empty;


    /// <summary>
    /// Usuario.
    /// </summary>
    private string Password { get; set; } = string.Empty;


    /// <summary>
    /// Código de la aplicación.
    /// </summary>
    private string AppCode { get; set; } = string.Empty;


    /// <summary>
    /// Modelo obtenido.
    /// </summary>
    public AccountModel? Account { get; set; } = null;




    /// <summary>
    /// Establecer credenciales.
    /// </summary>
    /// <param name="username">Usuario.</param>
    /// <param name="password">Contraseña.</param>
    /// <param name="appCode">Código de app.</param>
    public void SetCredentials(string username, string password, string appCode)
    {
        this.User = username;
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

// Si la contraseña es invalida.
        if (!password)
            return Responses.InvalidPassword;


        return Responses.Success;
    }



    /// <summary>
    /// Iniciar el proceso.
    /// </summary>
    private async Task<bool> GetAccount()
    {

        // Obtener la cuenta.
        var account = await accountData.Read(User, new()
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



    /// <summary>
    /// Obtener el token.
    /// </summary>
    public string GenerateToken() => JwtService.Generate(Account!, 0);



    /// <summary>
    /// Obtener el token.
    /// </summary>
    public AccountModel GetData() => Account!;

}