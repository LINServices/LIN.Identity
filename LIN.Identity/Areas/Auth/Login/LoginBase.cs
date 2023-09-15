namespace LIN.Identity.Areas.Auth.Login;


public abstract class LoginBase
{


    /// <summary>
    /// Llave de la aplicación
    /// </summary>
    private protected string ApplicationKey { get; set; }



    /// <summary>
    /// Tipo del login
    /// </summary>
    private protected LoginTypes LoginType { get; set; }



    /// <summary>
    /// Contraseña
    /// </summary>
    private protected string Password { get; set; }



    /// <summary>
    /// Modelo de la aplicación obtenida
    /// </summary>
    private protected ApplicationModel Application { get; set; }



    /// <summary>
    /// Datos de la cuenta
    /// </summary>
    private protected AccountModel Account { get; set; }



    /// <summary>
    /// Nuevo login
    /// </summary>
    /// <param name="account">Datos de la cuenta</param>
    /// <param name="application">Llave</param>
    public LoginBase(AccountModel? account, string? application, string password, LoginTypes loginType = LoginTypes.Credentials)
    {
        ApplicationKey = application ?? string.Empty;
        Account = account ?? new();
        Application = new();
        Password = password ?? string.Empty;
        LoginType = loginType;
    }




    /// <summary>
    /// Valida los datos obtenidos de la cuenta
    /// </summary>
    public ResponseBase Validate()
    {

        // Si la cuenta no esta activa
        if (Account.Estado != AccountStatus.Normal)
            return new()
            {
                Response = Responses.NotExistAccount,
                Message = "Esta cuenta fue eliminada o desactivada."
            };

        // Valida la contraseña
        if (Account.Contraseña != EncryptClass.Encrypt(Conexión.SecreteWord + Password))
            return new()
            {
                Response = Responses.InvalidPassword,
                Message = "La contraseña es incorrecta."
            };

        // Correcto
        return new(Responses.Success);

    }




    /// <summary>
    /// Valida los datos de la aplicación
    /// </summary>
    /// <param name="password">Contraseña</param>
    public async Task<ResponseBase> ValidateApp()
    {
        // Obtiene la App.
        var app = await Data.Applications.Read(ApplicationKey);

        // Verifica si la app existe.
        if (app.Response != Responses.Success)
            return new ReadOneResponse<AccountModel>
            {
                Message = "La aplicación no esta autorizada para iniciar sesión en LIN Identity",
                Response = Responses.Unauthorized
            };

        // Establece la aplicación
        Application = app.Model;

        // Correcto
        return new(Responses.Success);

    }



    /// <summary>
    /// Genera el login
    /// </summary>
    public void GenerateLogin()
    {
        // Crea registro del login
        _ = Logins.Create(new()
        {
            Date = DateTime.Now,
            AccountID = Account.ID,
            Type = LoginType,
            Application = new()
            {
                Key = ApplicationKey
            }
        });
    }



    /// <summary>
    /// Iniciar sesión
    /// </summary>
    public abstract Task<ResponseBase> Login();


}
