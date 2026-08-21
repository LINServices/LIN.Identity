namespace LIN.Cloud.Identity.Areas.Groups;

[IdentityToken]
[Route("[controller]")]
public class GroupsController(IGroupRepository groupData, IIamService rolesIam) : AuthenticationBaseController
{
    /// <summary>
    /// Crear nuevo grupo.
    /// </summary>
    /// <param name="group">Modelo del grupo.</param>
    [HttpPost]
    public async Task<HttpCreateResponse> Create([FromBody] GroupModel group)
    {

        var roles = await rolesIam.Validate(UserInformation.IdentityId, group.Identity.OwnerId ?? 0);

        bool iam = ValidateRoles.ValidateAlterMembers(roles);

        if (!iam)
            return new()
            {
                Message = "No tienes acceso para crear grupos.",
                Response = Responses.Unauthorized
            };

        // Formato de la identidad.
        group.Identity.Type = IdentityType.Group;
        Services.Formats.Identities.Process(group.Identity);

        var response = await groupData.Create(group);

        if (response.Response != Responses.Success)
            return new()
            {
                Response = response.Response
            };

        return new CreateResponse()
        {
            Response = Responses.Success,
            LastId = response.Model.Id
        };

    }

    /// <summary>
    /// Obtener todos los grupos de una organización.
    /// </summary>
    /// <param name="organization">Id de la organización.</param>
    /// <returns>Retorna la lista de grupos.</returns>
    [HttpGet]
    public async Task<HttpReadAllResponse<GroupModel>> ReadAll([FromQuery] int organization)
    {
        var roles = await rolesIam.Validate(UserInformation.IdentityId, organization);

        bool iam = ValidateRoles.ValidateRead(roles);

        if (!iam)
            return new()
            {
                Message = "No tienes acceso para leer grupos.",
                Response = Responses.Unauthorized
            };

        var response = await groupData.ReadAll(organization);

        if (response.Response != Responses.Success)
            return new()
            {
                Response = response.Response
            };

        return response;

    }

    /// <summary>
    /// Obtener un grupo.
    /// </summary>
    /// <param name="id">Id del grupo.</param>
    /// <returns>Retorna el modelo del grupo.</returns>
    [HttpGet("{id:int}")]
    public async Task<HttpReadOneResponse<GroupModel>> ReadOne([FromRoute] int id)
    {

        var orgId = await groupData.GetOwner(id);

        if (orgId.Response != Responses.Success)
            return new()
            {
                Message = "Hubo un error al encontrar la organización dueña de este grupo.",
                Response = Responses.Unauthorized
            };

        var roles = await rolesIam.Validate(UserInformation.IdentityId, orgId.Model);

        bool iam = ValidateRoles.ValidateRead(roles);

        if (!iam)
            return new()
            {
                Message = "No tienes acceso para leer grupos.",
                Response = Responses.Unauthorized
            };

        var response = await groupData.Read(id);

        if (response.Response != Responses.Success)
            return new()
            {
                Response = response.Response
            };

        return response;

    }

    /// <summary>
    /// Obtener un grupo.
    /// </summary>
    /// <param name="id">Id de la identidad del grupo.</param>
    /// <returns>Retorna el modelo del grupo.</returns>
    [HttpGet("identity/{id:int}")]
    public async Task<HttpReadOneResponse<GroupModel>> ReadIdentity([FromRoute] int id)
    {

        var orgId = await groupData.GetOwnerByIdentity(id);

        if (orgId.Response != Responses.Success)
            return new()
            {
                Message = "Hubo un error al encontrar la organización dueña de este grupo.",
                Response = Responses.Unauthorized
            };

        var roles = await rolesIam.Validate(UserInformation.IdentityId, orgId.Model);

        bool iam = ValidateRoles.ValidateRead(roles);

        if (!iam)
            return new()
            {
                Message = "No tienes acceso para leer grupos.",
                Response = Responses.Unauthorized
            };

        var response = await groupData.ReadByIdentity(id);

        if (response.Response != Responses.Success)
            return new()
            {
                Response = response.Response
            };

        return response;

    }

}