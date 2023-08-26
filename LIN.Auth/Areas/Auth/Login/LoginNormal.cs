namespace LIN.Auth.Areas.Auth.Login;


public abstract class LoginNormal : LoginBase
{


    /// <summary>
    /// Nuevo login
    /// </summary>
    /// <param name="account">Datos de la cuenta</param>
    /// <param name="application">Llave</param>
    public LoginNormal(AccountModel? account, string? application, string password) : base(account, application, password)
    {
    }



    /// <summary>
    /// Iniciar sesión
    /// </summary>
    public override async Task<ResponseBase> Login()
    {

        // Valida la aplicación
        var validateApp = await base.ValidateApp();

        // Retorna el error
        if (validateApp.Response != Responses.Success)
            return validateApp;

        // Validar credenciales y estado
        var validateAccount = Validate();

        // Retorna el error
        if (validateAccount.Response != Responses.Success)
            return validateApp;


        return new(Responses.Success);

    }


}
