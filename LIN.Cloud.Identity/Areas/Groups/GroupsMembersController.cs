namespace LIN.Cloud.Identity.Areas.Groups;

[IdentityToken]
[Route("Groups/members")]
public class GroupsMembersController(Data.Groups groupsData, Data.DirectoryMembers directoryMembersData, Data.GroupMembers groupMembers, IamRoles rolesIam) : AuthenticationBaseController
{

    /// <summary>
    /// Agregar un integrante a un grupo.
    /// </summary>
    /// <param name="model">Modelo del integrante.</param>
    /// <returns>Retorna el id del nuevo integrante.</returns>
    [HttpPost]
    public async Task<HttpCreateResponse> Create([FromBody] GroupMember model)
    {

        // Obtener la organización.
        var orgId = await groupsData.GetOwner(model.GroupId);

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
        bool iam = ValidateRoles.ValidateAlterMembers(roles);

        // Si no tiene permisos.
        if (!iam)
            return new()
            {
                Message = "No tienes acceso a la información este directorio.",
                Response = Responses.Unauthorized
            };

        // Valida si el usuario pertenece a la organización.
        var idIsIn = await directoryMembersData.IamIn(model.IdentityId, orgId.Model);

        // Si no existe.
        if (idIsIn.Response != Responses.Success)
            return new()
            {
                Message = $"La identidad {model.IdentityId} no pertenece al directorio de la organización {orgId.Model}.",
                Response = Responses.Unauthorized
            };

        // Crear el usuario.
        var response = await groupMembers.Create(model);

        // Retorna el resultado
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

        // Obtener la organización.
        var orgId = await groupsData.GetOwner(group);

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
        bool iam = ValidateRoles.ValidateAlterMembers(roles);

        // Si no tiene permisos.
        if (!iam)
            return new()
            {
                Message = "No tienes acceso a la información este directorio.",
                Response = Responses.Unauthorized
            };

        // Solo elementos distintos.
        ids = ids.Distinct().ToList();

        // Valida si el usuario pertenece a la organización.
        var (successIds, failureIds) = await directoryMembersData.IamIn(ids, orgId.Model);

        // Crear el usuario.
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

        response.Message = $"Se agregaron {successIds.Count} integrantes y se omitieron {failureIds.Count} debido a que no pertenecen a esta organización.";

        // Retorna el resultado
        return response;

    }


    /// <summary>
    /// Obtener los integrantes asociados a un grupo.
    /// </summary>
    /// <param name="group">ID del grupo.</param>
    [HttpGet("read/all")]
    public async Task<HttpReadAllResponse<GroupMember>> ReadMembers([FromQuery] int group)
    {

        // Obtener la organización.
        var orgId = await groupsData.GetOwner(group);

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
                Message = "No tienes acceso a la información este directorio.",
                Response = Responses.Unauthorized
            };


        // Obtiene el usuario.
        var response = await groupMembers.ReadAll(group);

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
    /// Buscar en los integrantes de un grupo.
    /// </summary>
    /// <param name="group">Grupo.</param>
    /// <param name="pattern">Patron de búsqueda.</param>
    [HttpGet("search")]
    public async Task<HttpReadAllResponse<IdentityModel>> Search([FromHeader] int group, [FromQuery] string pattern)
    {
        // Obtener la organización.
        var orgId = await groupsData.GetOwner(group);

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
                Message = "No tienes acceso a la información este directorio.",
                Response = Responses.Unauthorized
            };


        // Obtiene los miembros.
        var members = await groupMembers.Search(pattern, group);

        // Error al obtener los integrantes.
        if (members.Response != Responses.Success)
            return new()
            {
                Message = "Error.",
                Response = Responses.NotRows
            };

        // Retorna el resultado
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

        // Obtener la organización.
        var orgId = await groupsData.GetOwner(group);

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
                Message = "No tienes acceso a la información este directorio.",
                Response = Responses.Unauthorized
            };

        // Obtiene los miembros.
        var members = await groupMembers.SearchGroups(pattern, group);

        // Error al obtener los integrantes.
        if (members.Response != Responses.Success)
            return new()
            {
                Message = "Error.",
                Response = Responses.NotRows
            };

        // Retorna el resultado
        return members;

    }


    /// <summary>
    /// Eliminar un integrante
    /// </summary>
    /// <param name="group">ID del grupo.</param>
    [HttpDelete("remove")]
    public async Task<HttpResponseBase> DeleteMembers([FromQuery] int identity, [FromQuery] int group)
    {

        // Obtener la organización.
        var orgId = await groupsData.GetOwner(group);

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
        bool iam = ValidateRoles.ValidateAlterMembers(roles);

        // Si no tiene permisos.
        if (!iam)
            return new()
            {
                Message = "No tienes acceso a la información este directorio.",
                Response = Responses.Unauthorized
            };

        // Obtiene el usuario.
        var response = await groupMembers.Delete(identity, group);

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
    /// Obtener los grupos a los que una identidad pertenece.
    /// </summary>
    /// <param name="group">ID del grupo.</param>
    [HttpGet("read/on/all")]
    public async Task<HttpReadAllResponse<GroupModel>> OnMembers([FromQuery] int organization, [FromQuery] int identity)
    {

        // Confirmar el rol.
        var roles = await rolesIam.Validate(UserInformation.IdentityId, organization);

        // Iam.
        bool iam = ValidateRoles.ValidateRead(roles);

        // Si no tiene permisos.
        if (!iam)
            return new()
            {
                Message = "No tienes acceso a la información este directorio.",
                Response = Responses.Unauthorized
            };


        // Obtiene el usuario.
        var response = await groupMembers.OnMembers(organization, identity);

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