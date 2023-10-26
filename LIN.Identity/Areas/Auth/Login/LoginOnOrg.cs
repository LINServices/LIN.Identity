namespace LIN.Identity.Areas.Auth.Login;


public class LoginOnOrg : LoginService
{


    /// <summary>
    /// Acceso a la organización
    /// </summary>
    private OrganizationAccessModel OrganizationAccess { get; set; } = new();




    /// <summary>
    /// Nuevo login
    /// </summary>
    /// <param name="account">Datos de la cuenta</param>
    /// <param name="application">Llave</param>
    /// <param name="password">Contraseña</param>
    /// <param name="loginType">Tipo de inicio</param>
    public LoginOnOrg(AccountModel? account, string? application, string password, LoginTypes loginType = LoginTypes.Credentials) : base(account, application, password, loginType)
    {
    }



    /// <summary>
    /// Valida parámetros necesarios para iniciar sesión en una organización
    /// </summary>
    private bool ValidateParams()
    {
        return Account.OrganizationAccess != null;
    }



    /// <summary>
    /// Valida parámetros necesarios para iniciar sesión en una organización
    /// </summary>
    private async Task<bool> LoadOrganization()
    {

        var z = Account.OrganizationAccess?.Organization.ID;

        var orgResponse = await Data.Organizations.Organizations.Read(z ?? 0);


        if (orgResponse.Response != Responses.Success)
        {
            return false;
        }

        OrganizationAccess.Rol = Account.OrganizationAccess!.Rol;
        OrganizationAccess.ID = Account.OrganizationAccess.ID;
        OrganizationAccess.Organization = orgResponse.Model;

        return true;

    }



    /// <summary>
    /// Valida las políticas de la organización
    /// </summary>
    private async Task<ResponseBase> ValidatePolicies()
    {

        // Si el inicio de sesión fue desactivado por la organización
        if (!OrganizationAccess!.Organization.LoginAccess && !OrganizationAccess.Rol.IsAdmin())
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
                return new()
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

        return appOnOrg.Response == Responses.Success;

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


        // Validar la organización
        var validateAccess = await LoadOrganization();

        // Retorna el error
        if (!validateAccess)
            return new()
            {
                Message = "Hubo un error al validar la organización.",
                Response = Responses.Undefined
            };
 

        

        // Validar credenciales y estado
        var validateAccount = Validate();

        // Retorna el error
        if (validateAccount.Response != Responses.Success)
            return validateAccount;


        // Valida la aplicación
        var validateApp = await ValidateApp();

        // Retorna el error
        if (validateApp.Response != Responses.Success)
            return validateApp;

        // Validar las políticas
        var validatePolicies = await ValidatePolicies();

        // Retorna el error
        if (validatePolicies.Response != Responses.Success)
            return validatePolicies;

        // Genera el login
        GenerateLogin();

        return new(Responses.Success);

    }



}