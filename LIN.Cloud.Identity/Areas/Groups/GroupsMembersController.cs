namespace LIN.Cloud.Identity.Areas.Groups;

[IdentityToken]
[Route("Groups/members")]
public class GroupsMembersController(IGroupRepository groupsData, IOrganizationMemberRepository directoryMembersData, IGroupMemberRepository groupMembers, IIamService rolesIam, IOrganizationRepository organizationRepository) : AuthenticationBaseController
{
    /// <summary>
    /// Agregar un integrante a un grupo.
    /// </summary>
    /// <param name="model">Modelo del integrante.</param>
    /// <returns>Retorna el id del nuevo integrante.</returns>
    [HttpPost]
    public async Task<HttpCreateResponse> Create([FromBody] GroupMember model)
    {

        var orgId = await groupsData.GetOwner(model.GroupId);

        if (orgId.Response != Responses.Success)
            return new()
            {
                Message = "Hubo un error al encontrar la organización dueña de este grupo.",
                Response = Responses.Unauthorized
            };

        var roles = await rolesIam.Validate(UserInformation.IdentityId, orgId.Model);

        bool iam = ValidateRoles.ValidateAlterMembers(roles);

        if (!iam)
            return new()
            {
                Message = "No tienes acceso a la información este directorio.",
                Response = Responses.Unauthorized
            };

        // Valida si la identidad pertenece a la organización.
        var idIsIn = await directoryMembersData.IamIn(model.IdentityId, orgId.Model);

        // Si no existe.
        if (idIsIn.Response != Responses.Success)
            return new()
            {
                Message = $"La identidad {model.IdentityId} no pertenece al directorio de la organización {orgId.Model}.",
                Response = Responses.Unauthorized
            };

        var response = await groupMembers.Create(model);

        return response;

    }

    /// <summary>
    /// Agregar integrantes a un grupo.
    /// </summary>
    /// <param name="group">Id del grupo.</param>
    /// <param name="ids">Lista de las identidades.</param>
    /// <returns>Retorna la respuesta del proceso.</returns>
    [HttpPost("list")]
    public async Task<HttpCreateResponse> Create([FromHeader] int group, [FromBody] List<int> ids)
    {
        var orgId = await groupsData.GetOwner(group);

        if (orgId.Response != Responses.Success)
            return new()
            {
                Message = "Hubo un error al encontrar la organización dueña de este grupo.",
                Response = Responses.Unauthorized
            };

        var roles = await rolesIam.Validate(UserInformation.IdentityId, orgId.Model);

        bool iam = ValidateRoles.ValidateAlterMembers(roles);

        if (!iam)
            return new()
            {
                Message = "No tienes acceso a la información este directorio.",
                Response = Responses.Unauthorized
            };

        ids = ids.Distinct().ToList();

        // Valida si la identidad pertenece a la organización.
        var (successIds, failureIds) = await directoryMembersData.IamIn(ids, orgId.Model);

        var response = await groupMembers.Create(successIds.Select(id => new GroupMember
        {
            Group = new()
            {
                Id = group,
            },
            Identity = new()
            {
                Id = id
            }
        }));

        response.Message = $"Se agregaron {successIds.Count()} integrantes y se omitieron {failureIds.Count} debido a que no pertenecen a esta organización.";

        return response;
    }

    /// <summary>
    /// Obtener los integrantes asociados a un grupo.
    /// </summary>
    /// <param name="group">ID del grupo.</param>
    [HttpGet("read/all")]
    public async Task<HttpReadAllResponse<GroupMember>> ReadMembers([FromQuery] int group)
    {

        var orgId = await groupsData.GetOwner(group);

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
                Message = "No tienes acceso a la información este directorio.",
                Response = Responses.Unauthorized
            };


        var response = await groupMembers.ReadAll(group);

        if (response.Response != Responses.Success)
            return new()
            {
                Response = response.Response
            };

        return response;

    }

    /// <summary>
    /// Buscar en los integrantes de un grupo.
    /// </summary>
    /// <param name="group">Grupo.</param>
    /// <param name="pattern">Patron de búsqueda.</param>
    [HttpGet("search")]
    public async Task<HttpReadAllResponse<IdentityModel>> Search([FromHeader] int group, [FromQuery] string pattern)
    {
        var orgId = await groupsData.GetOwner(group);

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
                Message = "No tienes acceso a la información este directorio.",
                Response = Responses.Unauthorized
            };


        var members = await groupMembers.Search(pattern, group);

        if (members.Response != Responses.Success)
            return new()
            {
                Message = "Error.",
                Response = Responses.NotRows
            };

        return members;

    }

    /// <summary>
    /// Buscar en los grupos de un grupo.
    /// </summary>
    /// <param name="group">Grupo.</param>
    /// <param name="pattern">Patron de búsqueda.</param>
    [HttpGet("search/groups")]
    public async Task<HttpReadAllResponse<IdentityModel>> SearchOnGroups([FromHeader] int group, [FromQuery] string pattern)
    {

        var orgId = await groupsData.GetOwner(group);

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
                Message = "No tienes acceso a la información este directorio.",
                Response = Responses.Unauthorized
            };

        var members = await groupMembers.Search(pattern, group);

        if (members.Response != Responses.Success)
            return new()
            {
                Message = "Error.",
                Response = Responses.NotRows
            };

        return members;

    }

    /// <summary>
    /// Eliminar un integrante
    /// </summary>
    /// <param name="group">ID del grupo.</param>
    [HttpDelete("remove")]
    public async Task<HttpResponseBase> DeleteMembers([FromQuery] int identity, [FromQuery] int group)
    {
        var orgId = await groupsData.GetOwner(group);

        if (orgId.Response != Responses.Success)
            return new()
            {
                Message = "Hubo un error al encontrar la organización dueña de este grupo.",
                Response = Responses.Unauthorized
            };

        var roles = await rolesIam.Validate(UserInformation.IdentityId, orgId.Model);

        bool iam = ValidateRoles.ValidateAlterMembers(roles);

        if (!iam)
            return new()
            {
                Message = "No tienes acceso a la información este directorio.",
                Response = Responses.Unauthorized
            };

        // Obtener el directorio principal de la organización.
        var dictoryId = await organizationRepository.ReadDirectory(orgId.Model);

        // Si hubo un error al obtener el directorio principal.
        if (dictoryId.Response != Responses.Success)
            return new()
            {
                Message = "Hubo un error al encontrar el directorio principal de la organización.",
                Response = Responses.Unauthorized
            };

        // Si el grupo es el directorio principal de la organización, se expulsa a la identidad de la organización.
        if (group == dictoryId.Model)
        {
            // Expulsar de la organización requiere permisos de eliminación.
            bool canDelete = ValidateRoles.ValidateDelete(roles);

            if (!canDelete)
                return new()
                {
                    Message = "No tienes autorización para eliminar integrantes del directorio principal de la organización.",
                    Response = Responses.Unauthorized
                };

            // Expulsar la identidad de la organización (desactiva la cuenta, elimina sus roles y sus membresías en los grupos).
            var expulseResponse = await directoryMembersData.Expulse([identity], orgId.Model);

            // Si es erróneo.
            if (expulseResponse.Response != Responses.Success)
                return new()
                {
                    Response = expulseResponse.Response
                };

        }

        var response = await groupMembers.Delete(identity, group);

        if (response.Response != Responses.Success)
            return new()
            {
                Response = response.Response
            };

        return response;
    }

    /// <summary>
    /// Obtener los grupos a los que una identidad pertenece.
    /// </summary>
    /// <param name="group">ID del grupo.</param>
    [HttpGet("read/on/all")]
    public async Task<HttpReadAllResponse<GroupModel>> OnMembers([FromQuery] int organization, [FromQuery] int identity)
    {

        var roles = await rolesIam.Validate(UserInformation.IdentityId, organization);

        bool iam = ValidateRoles.ValidateRead(roles);

        if (!iam)
            return new()
            {
                Message = "No tienes acceso a la información este directorio.",
                Response = Responses.Unauthorized
            };


        var response = await groupMembers.OnMembers(organization, identity);

        if (response.Response != Responses.Success)
            return new()
            {
                Response = response.Response
            };

        return response;

    }

}