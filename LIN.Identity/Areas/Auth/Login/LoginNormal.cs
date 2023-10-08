namespace LIN.Identity.Areas.Auth.Login;


public class LoginNormal : LoginBase
{


    /// <summary>
    /// Nuevo login
    /// </summary>
    /// <param name="account">Datos de la cuenta</param>
    /// <param name="application">Llave</param>
    /// <param name="password">Contraseña</param>
    /// <param name="loginType">Tipo de inicio</param>
    public LoginNormal(AccountModel? account, string? application, string password, LoginTypes loginType = LoginTypes.Credentials) : base(account, application, password, loginType)
    {
    }



    /// <summary>
    /// Iniciar sesión
    /// </summary>
    public override async Task<ResponseBase> Login()
    {

        // Valida la aplicación
        var validateApp = await ValidateApp();

        // Retorna el error
        if (validateApp.Response != Responses.Success)
            return validateApp;

        // Validar credenciales y estado
        var validateAccount = Validate();

        // Retorna el error
        if (validateAccount.Response != Responses.Success)
            return validateAccount;

        // Genera el login
        GenerateLogin();

        return new(Responses.Success);

    }


}