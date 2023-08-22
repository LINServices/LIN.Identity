namespace LIN.Auth.Controllers;


[Route("orgs")]
public class OrganizationsController : ControllerBase
{


    /// <summary>
    /// Crea una organizaci�n
    /// </summary>
    /// <param name="modelo">Modelo de la organizaci�n</param>
    [HttpPost("create")]
    public async Task<HttpCreateResponse> Create([FromBody] OrganizationModel modelo, [FromHeader] string token)
    {

        // Comprobaciones
        if (modelo == null || modelo.Domain.Length <= 0 || modelo.Name.Length <= 0)
            return new(Responses.InvalidParam);



        // Token
        var (isValid, _, userID) = Jwt.Validate(token);

        // Validaci�n del token
        if (!isValid)
            return new CreateResponse()
            {
                Response = Responses.Unauthorized,
                Message = "Token invalido"
            };


        // Conexi�n
        (Conexi�n context, string connectionKey) = Conexi�n.GetOneConnection();

        // Obtiene la cuenta
        var account = await Data.Accounts.Read(userID, true, true, true, context);

        // Validaci�n de la cuenta
        if (account.Response != Responses.Success)
        {
            return new CreateResponse()
            {
                Response = Responses.Unauthorized,
                Message = "No se encontr� el usuario"
            };
        }


        // Si ya el usuario tiene organizaci�n
        if (account.Model.OrganizationAccess != null)
        {
            return new CreateResponse()
            {
                Response = Responses.UnauthorizedByOrg,
                Message = "Ya perteneces a una organizaci�n."
            };
        }


        // Organizaci�n del modelo
        modelo.ID = 0;
        modelo.AppList = new();
        modelo.Members = new();

        // Creaci�n de la organizaci�n
        var response = await Data.Organizations.Create(modelo, userID, context);

        // Evaluaci�n
        if (response.Response != Responses.Success)
            return new(response.Response);

        context.CloseActions(connectionKey);

        // Retorna el resultado
        return new CreateResponse()
        {
            LastID = response.Model.ID,
            Response = Responses.Success,
            Message = "Success"
        };

    }



    /// <summary>
    /// Obtiene una organizaci�n por medio del ID
    /// </summary>
    /// <param name="id">ID de la organizaci�n</param>
    [HttpGet("read/id")]
    public async Task<HttpReadOneResponse<OrganizationModel>> ReadOneByID([FromQuery] int id)
    {

        if (id <= 0)
            return new(Responses.InvalidParam);

        // Obtiene el usuario
        var response = await Data.Organizations.Read(id);

        // Si es err�neo
        if (response.Response != Responses.Success)
            return new ReadOneResponse<OrganizationModel>()
            {
                Response = response.Response,
                Model = new()
            };

        // Retorna el resultado
        return response;

    }



    /// <summary>
    /// Crea una cuenta en una organizaci�n
    /// </summary>
    /// <param name="modelo">Modelo del usuario</param>
    [HttpPost("create/member")]
    public async Task<HttpCreateResponse> Create([FromBody] AccountModel modelo, [FromHeader] string token, [FromHeader] OrgRoles rol)
    {

        // Validaci�n del modelo.
        if (modelo == null || !modelo.Usuario.Trim().Any() || !modelo.Nombre.Trim().Any())
        {
            return new CreateResponse
            {
                Response = Responses.InvalidParam,
                Message = "Uno o varios par�metros inv�lidos."
            };
        }

        // Organizaci�n del modelo
        modelo.ID = 0;
        modelo.Creaci�n = DateTime.Now;
        modelo.Estado = AccountStatus.Normal;
        modelo.Insignia = AccountBadges.None;
        modelo.Rol = AccountRoles.User;
        modelo.OrganizationAccess = null;
        modelo.Perfil = modelo.Perfil.Length == 0
                               ? System.IO.File.ReadAllBytes("wwwroot/profile.png")
                               : modelo.Perfil;


        // Establece la contrase�a default
        string password = $"ChangePwd@{modelo.Creaci�n:dd.MM.yyyy}";

        // Contrase�a default
        modelo.Contrase�a = EncryptClass.Encrypt(Conexi�n.SecreteWord + password);

        // Validaci�n del token
        var (isValid, _, userID) = Jwt.Validate(token);

        // Token es invalido
        if (!isValid)
        {
            return new CreateResponse
            {
                Message = "Token invalido.",
                Response = Responses.Unauthorized
            };
        }


        // Obtiene el usuario
        var userContext = await Data.Accounts.Read(userID, true, false, true);

        // Error al encontrar el usuario
        if (userContext.Response != Responses.Success)
        {
            return new CreateResponse
            {
                Message = "No se encontr� un usuario valido.",
                Response = Responses.Unauthorized
            };
        }

        // Si el usuario no tiene una organizaci�n
        if (userContext.Model.OrganizationAccess == null)
        {
            return new CreateResponse
            {
                Message = $"El usuario '{userContext.Model.Usuario}' no pertenece a una organizaci�n.",
                Response = Responses.Unauthorized
            };
        }

        // Verificaci�n del rol dentro de la organizaci�n
        if (!userContext.Model.OrganizationAccess.Rol.IsAdmin())
        {
            return new CreateResponse
            {
                Message = $"El usuario '{userContext.Model.Usuario}' no puede crear nuevos usuarios en esta organizaci�n.",
                Response = Responses.Unauthorized
            };
        }


        // Verificaci�n del rol dentro de la organizaci�n
        if (userContext.Model.OrganizationAccess.Rol.IsGretter(rol))
        {
            return new CreateResponse
            {
                Message = $"El '{userContext.Model.Usuario}' no puede crear nuevos usuarios con mas privilegios de los propios.",
                Response = Responses.Unauthorized
            };
        }




        // ID de la organizaci�n
        var org = userContext.Model.OrganizationAccess.Organization.ID;


        // Conexi�n
        (Conexi�n context, string connectionKey) = Conexi�n.GetOneConnection();

        // Creaci�n del usuario
        var response = await Data.Accounts.Create(modelo, org, rol, context);

        // Evaluaci�n
        if (response.Response != Responses.Success)
            return new(response.Response);

        // Cierra la conexi�n
        context.CloseActions(connectionKey);

        // Retorna el resultado
        return new CreateResponse()
        {
            LastID = response.Model.ID,
            Response = Responses.Success,
            Message = "Success"
        };

    }











    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelo">Modelo del usuario</param>
    [HttpGet("members")]
    public async Task<HttpReadAllResponse<AccountModel>> Create([FromHeader] string token)
    {

        var (isValid, _, userID) = Jwt.Validate(token);


        if (!isValid)
        {
            return new ReadAllResponse<AccountModel>
            {
                Message = "",
                Response = Responses.Unauthorized
            };
        }


        var org = await Data.Organizations.ReadMembers(userID);


        if (org.Response != Responses.Success)
        {
            return new ReadAllResponse<AccountModel>
            {
                Message = "No found Organization",
                Response = Responses.Unauthorized
            };
        }



        // Conexi�n
        (Conexi�n context, string connectionKey) = Conexi�n.GetOneConnection();

        context.CloseActions(connectionKey);

        // Retorna el resultado
        return org;

    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelo">Modelo del usuario</param>
    [HttpGet("apps")]
    public async Task<HttpReadAllResponse<ApplicationModel>> w([FromHeader] string token)
    {

        var (isValid, _, userID) = Jwt.Validate(token);


        if (!isValid)
        {
            return new ReadAllResponse<ApplicationModel>
            {
                Message = "",
                Response = Responses.Unauthorized
            };
        }


        var org = await Data.Organizations.ReadApps(userID);


        if (org.Response != Responses.Success)
        {
            return new ReadAllResponse<ApplicationModel>
            {
                Message = "No found Organization",
                Response = Responses.Unauthorized
            };
        }



        // Conexi�n
        (Conexi�n context, string connectionKey) = Conexi�n.GetOneConnection();

        context.CloseActions(connectionKey);

        // Retorna el resultado
        return org;

    }


}