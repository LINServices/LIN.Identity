namespace LIN.Auth.Areas.Organizations;


[Route("orgs")]
public class OrganizationsController : ControllerBase
{


    /// <summary>
    /// Crea una nueva organización
    /// </summary>
    /// <param name="modelo">Modelo de la organización y el usuario administrador</param>
    [HttpPost("create")]
    public async Task<HttpCreateResponse> Create([FromBody] OrganizationModel modelo)
    {

        // Comprobaciones
        if (modelo == null || modelo.Domain.Length <= 0 || modelo.Name.Length <= 0 || modelo.Members.Count <= 0)
            return new(Responses.InvalidParam);



        // Conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();


        // Organización del modelo
        modelo.ID = 0;
        modelo.AppList = new();

        modelo.Members[0].Member = LIN.Auth.Controllers.Processors.AccountProcessor.Process(modelo.Members[0].Member);
        foreach (var member in modelo.Members)
        {
            member.Rol = OrgRoles.SuperManager;
            member.Organization = modelo;
        }

        // Creación de la organización
        var response = await Data.Organizations.Organizations.Create(modelo, context);

        // Evaluación
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
    /// Obtiene una organización por medio del ID
    /// </summary>
    /// <param name="id">ID de la organización</param>
    [HttpGet("read/id")]
    public async Task<HttpReadOneResponse<OrganizationModel>> ReadOneByID([FromQuery] int id)
    {

        if (id <= 0)
            return new(Responses.InvalidParam);

        // Obtiene el usuario
        var response = await Data.Organizations.Organizations.Read(id);

        // Si es erróneo
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
    /// Actualiza si una organización tiene lista blanca
    /// </summary>
    /// <param name="token">Toke de acceso administrador</param>
    /// <param name="haveWhite">Nuevo estado</param>
    [HttpPatch("update/whitelist")]
    public async Task<HttpResponseBase> Update([FromHeader] string token, [FromQuery] bool haveWhite)
    {


        var (isValid, _, userID, _) = Jwt.Validate(token);


        if (!isValid)
            return new(Responses.Unauthorized);


        var userContext = await Data.Accounts.Read(userID, true, true, true);

        // Error al encontrar el usuario
        if (userContext.Response != Responses.Success)
        {
            return new ResponseBase
            {
                Message = "No se encontró un usuario valido.",
                Response = Responses.Unauthorized
            };
        }

        // Si el usuario no tiene una organización
        if (userContext.Model.OrganizationAccess == null)
        {
            return new ResponseBase
            {
                Message = $"El usuario '{userContext.Model.Usuario}' no pertenece a una organización.",
                Response = Responses.Unauthorized
            };
        }

        // Verificación del rol dentro de la organización
        if (!userContext.Model.OrganizationAccess.Rol.IsAdmin())
        {
            return new ResponseBase
            {
                Message = $"El usuario '{userContext.Model.Usuario}' no puede actualizar el estado de la lista blanca de esta organización.",
                Response = Responses.Unauthorized
            };
        }


        var response = await Data.Organizations.Organizations.UpdateState(userContext.Model.OrganizationAccess.Organization.ID, haveWhite);

        // Retorna el resultado
        return response;

    }



    /// <summary>
    /// Actualiza si los usuarios no admins de una organización tienen acceso a su cuenta
    /// </summary>
    /// <param name="token">Token de acceso administrador</param>
    /// <param name="state">Nuevo estado</param>
    [HttpPatch("update/access")]
    public async Task<HttpResponseBase> UpdateAccess([FromHeader] string token, [FromQuery] bool state)
    {


        var (isValid, _, userID, _) = Jwt.Validate(token);


        if (!isValid)
            return new(Responses.Unauthorized);


        var userContext = await Data.Accounts.Read(userID, true, false, true);

        // Error al encontrar el usuario
        if (userContext.Response != Responses.Success)
        {
            return new ResponseBase
            {
                Message = "No se encontró un usuario valido.",
                Response = Responses.Unauthorized
            };
        }

        // Si el usuario no tiene una organización
        if (userContext.Model.OrganizationAccess == null)
        {
            return new ResponseBase
            {
                Message = $"El usuario '{userContext.Model.Usuario}' no pertenece a una organización.",
                Response = Responses.Unauthorized
            };
        }

        // Verificación del rol dentro de la organización
        if (userContext.Model.OrganizationAccess.Rol != OrgRoles.SuperManager)
        {
            return new ResponseBase
            {
                Message = $"El usuario '{userContext.Model.Usuario}' no puede actualizar el estado de accesos de esta organización.",
                Response = Responses.Unauthorized
            };
        }


        var response = await Data.Organizations.Organizations.UpdateAccess(userContext.Model.OrganizationAccess.Organization.ID, state);

        // Retorna el resultado
        return response;

    }


}