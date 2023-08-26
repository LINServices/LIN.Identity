﻿namespace LIN.Auth.Areas.Auth.Login;


public class LoginNormal : LoginBase
{


    /// <summary>
    /// Nuevo login
    /// </summary>
    /// <param name="account">Datos de la cuenta</param>
    /// <param name="application">Llave</param>
    public LoginNormal(AccountModel? account, string? application, string password, LoginTypes loginType) : base(account, application, password, loginType)
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

        // Genera el login
        base.GenerateLogin();

        return new(Responses.Success);

    }


}
