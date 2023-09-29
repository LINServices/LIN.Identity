using LIN.Identity.Controllers.Processors;
namespace LIN.Identity.Areas.Organizations;


[Route("orgs")]
public class OrganizationsController : ControllerBase
{


    /// <summary>
    /// Crea una nueva organizaci�n
    /// </summary>
    /// <param name="modelo">Modelo de la organizaci�n y el usuario administrador</param>
    [HttpPost("create")]
    public async Task<HttpCreateResponse> Create([FromBody] OrganizationModel modelo)
    {

        // Comprobaciones
        if (modelo == null || modelo.Domain.Length <= 0 || modelo.Name.Length <= 0 || modelo.Members.Count <= 0)
            return new(Responses.InvalidParam);


        // Conexi�n
        var (context, connectionKey) = Conexión.GetOneConnection();


        // Organizaci�n del modelo
        modelo.ID = 0;
        modelo.AppList = new();

        modelo.Members[0].Member = AccountProcessor.Process(modelo.Members[0].Member);
        foreach (var member in modelo.Members)
        {
            member.Rol = OrgRoles.SuperManager;
            member.Organization = modelo;
        }

        // Creaci�n de la organizaci�n
        var response = await Data.Organizations.Organizations.Create(modelo, context);

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
        var response = await Data.Organizations.Organizations.Read(id);

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
    /// Actualiza si una organizaci�n tiene lista blanca
    /// </summary>
    /// <param name="token">Toke de acceso administrador</param>
    /// <param name="haveWhite">Nuevo estado</param>
    [HttpPatch("update/whitelist")]
    public async Task<HttpResponseBase> Update([FromHeader] string token, [FromQuery] bool haveWhite)
    {


        var (isValid, _, userID, _, _) = Jwt.Validate(token);


        if (!isValid)
            return new(Responses.Unauthorized);


        var userContext = await Data.Accounts.ReadBasic(userID);

        // Error al encontrar el usuario
        if (userContext.Response != Responses.Success)
        {
            return new ResponseBase
            {
                Message = "No se encontr� un usuario valido.",
                Response = Responses.Unauthorized
            };
        }

        // Si el usuario no tiene una organizaci�n
        if (userContext.Model.OrganizationAccess == null)
        {
            return new ResponseBase
            {
                Message = $"El usuario '{userContext.Model.Usuario}' no pertenece a una organizaci�n.",
                Response = Responses.Unauthorized
            };
        }

        // Verificaci�n del rol dentro de la organizaci�n
        if (!userContext.Model.OrganizationAccess.Rol.IsAdmin())
        {
            return new ResponseBase
            {
                Message = $"El usuario '{userContext.Model.Usuario}' no puede actualizar el estado de la lista blanca de esta organizaci�n.",
                Response = Responses.Unauthorized
            };
        }


        var response = await Data.Organizations.Organizations.UpdateState(userContext.Model.OrganizationAccess.Organization.ID, haveWhite);

        // Retorna el resultado
        return response;

    }



    /// <summary>
    /// Actualiza si los usuarios no admins de una organizaci�n tienen acceso a su cuenta
    /// </summary>
    /// <param name="token">Token de acceso administrador</param>
    /// <param name="state">Nuevo estado</param>
    [HttpPatch("update/access")]
    public async Task<HttpResponseBase> UpdateAccess([FromHeader] string token, [FromQuery] bool state)
    {


        var (isValid, _, userID, _, _) = Jwt.Validate(token);


        if (!isValid)
            return new(Responses.Unauthorized);


        var userContext = await Data.Accounts.ReadBasic(userID);

        // Error al encontrar el usuario
        if (userContext.Response != Responses.Success)
        {
            return new ResponseBase
            {
                Message = "No se encontr� un usuario valido.",
                Response = Responses.Unauthorized
            };
        }

        // Si el usuario no tiene una organizaci�n
        if (userContext.Model.OrganizationAccess == null)
        {
            return new ResponseBase
            {
                Message = $"El usuario '{userContext.Model.Usuario}' no pertenece a una organizaci�n.",
                Response = Responses.Unauthorized
            };
        }

        // Verificaci�n del rol dentro de la organizaci�n
        if (userContext.Model.OrganizationAccess.Rol != OrgRoles.SuperManager)
        {
            return new ResponseBase
            {
                Message = $"El usuario '{userContext.Model.Usuario}' no puede actualizar el estado de accesos de esta organizaci�n.",
                Response = Responses.Unauthorized
            };
        }


        var response = await Data.Organizations.Organizations.UpdateAccess(userContext.Model.OrganizationAccess.Organization.ID, state);

        // Retorna el resultado
        return response;

    }


}