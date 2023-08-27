namespace LIN.Auth.Areas.Auth.Login;


public class LoginOnOrg : LoginBase
{


    /// <summary>
    /// Acceso a la organización
    /// </summary>
    private OrganizationAccessModel? OrganizationAccess => base.Account.OrganizationAccess;




    /// <summary>
    /// Nuevo login
    /// </summary>
    /// <param name="account">Datos de la cuenta</param>
    /// <param name="application">Llave</param>
    public LoginOnOrg(AccountModel? account, string? application, string password, LoginTypes loginType = LoginTypes.Credentials) : base(account, application, password, loginType)
    {
    }



    /// <summary>
    /// Valida parámetros necesarios para iniciar sesión en una organización
    /// </summary>
    private bool ValidateParams()
    {
        return (OrganizationAccess != null);
    }



    /// <summary>
    /// Valida las políticas de la organización
    /// </summary>
    private async Task<ResponseBase> ValidatePolicies()
    {

        // Si el inicio de sesión fue desactivado por la organización
        if (!OrganizationAccess!.Organization.LoginAccess && OrganizationAccess.Rol.IsAdmin())
            return new()
            {
                Message = "Tu organización a deshabilitado el inicio de sesión temporalmente.",
                Response = Responses.LoginBlockedByOrg
            };


        // Si la organización tiene lista blanca
        if (OrganizationAccess.Organization.HaveWhiteList)
        {
            var whiteList = await ValidateWhiteList();
            if (!whiteList)
                return new ()
                {
                    Message = "Tu organización no permite iniciar sesión en esta aplicación.",
                    Response = Responses.UnauthorizedByOrg
                };
        }

        return new(Responses.Success);

    }



    /// <summary>
    /// Valida la app en la lista blanca
    /// </summary>
    private async Task<bool> ValidateWhiteList()
    {

        // Busca la app en la organización
        var appOnOrg = await Data.Organizations.Applications.AppOnOrg(ApplicationKey, OrganizationAccess!.Organization.ID);

        return (appOnOrg.Response == Responses.Success);

    }



    /// <summary>
    /// Iniciar sesión
    /// </summary>
    public override async Task<ResponseBase> Login()
    {


        // Valida la aplicación
        var validateParams = ValidateParams();

        // Retorna el error
        if (!validateParams)
            return new()
            {
                Message = "Este usuario no pertenece a una organización.",
                Response = Responses.Undefined
            };


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

        // Validar las políticas
        var validatePolicies = await ValidatePolicies();

        // Retorna el error
        if (validatePolicies.Response != Responses.Success)
            return validateApp;

        // Genera el login
        base.GenerateLogin();

        return new(Responses.Success);

    }



}
