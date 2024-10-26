namespace LIN.Cloud.Identity.Areas.Groups;

[IdentityToken]
[Route("[controller]")]
public class GroupsController(Data.Groups groupData, IamRoles rolesIam) : AuthenticationBaseController
{

    /// <summary>
    /// Crear nuevo grupo.
    /// </summary>
    /// <param name="group">Modelo del grupo.</param>
    [HttpPost]
    public async Task<HttpCreateResponse> Create([FromBody] GroupModel group)
    {

        // Confirmar el rol.
        var roles = await rolesIam.Validate(UserInformation.IdentityId, group.OwnerId ?? 0);

        // Iam.
        bool iam = ValidateRoles.ValidateAlterMembers(roles);

        // Si no tiene permisos.
        if (!iam)
            return new()
            {
                Message = "No tienes acceso para crear grupos.",
                Response = Responses.Unauthorized
            };

        // Formato de la identidad.
        group.Identity.Type = IdentityType.Group;
        Services.Formats.Identities.Process(group.Identity);

        // Obtener el modelo.
        var response = await groupData.Create(group);

        // Si es erróneo
        if (response.Response != Responses.Success)
            return new()
            {
                Response = response.Response
            };

        // Retorna el resultado
        return new CreateResponse()
        {
            Response = Responses.Success,
            LastID = response.Model.Id
        };

    }


    /// <summary>
    /// Obtener todos los grupos de una organización.
    /// </summary>
    /// <param name="organization">Id de la organización.</param>
    /// <returns>Retorna la lista de grupos.</returns>
    [HttpGet("all")]
    public async Task<HttpReadAllResponse<GroupModel>> ReadAll([FromHeader] int organization)
    {
        // Confirmar el rol.
        var roles = await rolesIam.Validate(UserInformation.IdentityId, organization);

        // Iam.
        bool iam = ValidateRoles.ValidateRead(roles);

        // Si no tiene permisos.
        if (!iam)
            return new()
            {
                Message = "No tienes acceso para leer grupos.",
                Response = Responses.Unauthorized
            };

        // Obtener el modelo.
        var response = await groupData.ReadAll(organization);

        // Si es erróneo
        if (response.Response != Responses.Success)
            return new()
            {
                Response = response.Response
            };

        // Retorna el resultado
        return response;

    }


    /// <summary>
    /// Obtener un grupo.
    /// </summary>
    /// <param name="id">Id del grupo.</param>
    /// <returns>Retorna el modelo del grupo.</returns>
    [HttpGet]
    public async Task<HttpReadOneResponse<GroupModel>> ReadOne([FromHeader] int id)
    {

        // Obtener la organización.
        var orgId = await groupData.GetOwner(id);

        // Si hubo un error.
        if (orgId.Response != Responses.Success)
            return new()
            {
                Message = "Hubo un error al encontrar la organización dueña de este grupo.",
                Response = Responses.Unauthorized
            };

        // Confirmar el rol.
        var roles = await rolesIam.Validate(UserInformation.IdentityId, orgId.Model);

        // Iam.
        bool iam = ValidateRoles.ValidateRead(roles);

        // Si no tiene permisos.
        if (!iam)
            return new()
            {
                Message = "No tienes acceso para leer grupos.",
                Response = Responses.Unauthorized
            };

        // Obtener el modelo.
        var response = await groupData.Read(id);

        // Si es erróneo
        if (response.Response != Responses.Success)
            return new()
            {
                Response = response.Response
            };

        // Retorna el resultado
        return response;

    }


    /// <summary>
    /// Obtener un grupo.
    /// </summary>
    /// <param name="id">Id de la identidad del grupo.</param>
    /// <returns>Retorna el modelo del grupo.</returns>
    [HttpGet("identity")]
    public async Task<HttpReadOneResponse<GroupModel>> ReadIdentity([FromHeader] int id)
    {

        // Obtener la organización.
        var orgId = await groupData.GetOwnerByIdentity(id);

        // Si hubo un error.
        if (orgId.Response != Responses.Success)
            return new()
            {
                Message = "Hubo un error al encontrar la organización dueña de este grupo.",
                Response = Responses.Unauthorized
            };

        // Confirmar el rol.
        var roles = await rolesIam.Validate(UserInformation.IdentityId, orgId.Model);

        // Iam.
        bool iam = ValidateRoles.ValidateRead(roles);

        // Si no tiene permisos.
        if (!iam)
            return new()
            {
                Message = "No tienes acceso para leer grupos.",
                Response = Responses.Unauthorized
            };

        // Obtener el modelo.
        var response = await groupData.ReadByIdentity(id);

        // Si es erróneo
        if (response.Response != Responses.Success)
            return new()
            {
                Response = response.Response
            };

        // Retorna el resultado
        return response;

    }

}