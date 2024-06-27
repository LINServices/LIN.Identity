namespace LIN.Cloud.Identity.Areas.Groups;


[Route("Groups/members")]
public class GroupsMembersController(Data.Groups groupsData, Data.DirectoryMembers directoryMembersData, Data.GroupMembers groupMembers, RolesIam rolesIam) : Controller
{


    /// <summary>
    /// Crear nuevo integrante.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    /// <param name="model">Modelo.</param>
    [HttpPost]
    [IdentityToken]
    public async Task<HttpCreateResponse> Create([FromHeader] string token, [FromBody] GroupMember model)
    {

        // Token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();

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
        var (_, roles) = await rolesIam.RolesOn(tokenInfo.IdentityId, orgId.Model);

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
    /// Crear nuevos integrantes en un grupo.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    /// <param name="group">Id del grupo.</param>
    /// <param name="ids">Identidades.</param>
    [HttpPost("list")]
    [IdentityToken]
    public async Task<HttpCreateResponse> Create([FromHeader] string token, [FromHeader] int group, [FromBody] List<int> ids)
    {

        // Token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();

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
        var (_, roles) = await rolesIam.RolesOn(tokenInfo.IdentityId, orgId.Model);

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
        var idIsIn = await directoryMembersData.IamIn(ids, orgId.Model);

        // Si no existe.
        if (idIsIn.Response != Responses.Success)
            return new()
            {
                Message = $"Errores encontrados al validar si las identidades pertenecen a la organización {orgId.Model}.",
                Response = Responses.Unauthorized
            };


        // Crear el usuario.
        var response = await groupMembers.Create(ids.Select(id => new GroupMember
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

        // Retorna el resultado
        return response;

    }



    /// <summary>
    /// Obtener los integrantes asociados a un grupo.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    /// <param name="group">ID del grupo.</param>
    [HttpGet("read/all")]
    [IdentityToken]
    public async Task<HttpReadAllResponse<GroupMember>> ReadMembers([FromHeader] string token, [FromQuery] int group)
    {

        // Token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();

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
        var (_, roles) = await rolesIam.RolesOn(tokenInfo.IdentityId, orgId.Model);

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
    /// <param name="token">Token de acceso.</param>
    /// <param name="group">Grupo.</param>
    /// <param name="pattern">Patron de búsqueda.</param>
    [HttpGet("search")]
    [IdentityToken]
    public async Task<HttpReadAllResponse<IdentityModel>> Search([FromHeader] string token, [FromHeader] int group, [FromQuery] string pattern)
    {

        // Información del token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();

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
        var (_, roles) = await rolesIam.RolesOn(tokenInfo.IdentityId, orgId.Model);

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
    /// <param name="token">Token de acceso.</param>
    /// <param name="group">Grupo.</param>
    /// <param name="pattern">Patron de búsqueda.</param>
    [HttpGet("search/groups")]
    [IdentityToken]
    public async Task<HttpReadAllResponse<IdentityModel>> SearchOnGroups([FromHeader] string token, [FromHeader] int group, [FromQuery] string pattern)
    {

        // Información del token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();

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
        var (_, roles) = await rolesIam.RolesOn(tokenInfo.IdentityId, orgId.Model);

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
    /// <param name="token">Token de acceso.</param>
    /// <param name="group">ID del grupo.</param>
    [HttpDelete("remove")]
    [IdentityToken]
    public async Task<HttpResponseBase> DeleteMembers([FromHeader] string token, [FromQuery] int identity, [FromQuery] int group)
    {

        // Token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();

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
        var (_, roles) = await rolesIam.RolesOn(tokenInfo.IdentityId, orgId.Model);

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
    /// <param name="token">Token de acceso.</param>
    /// <param name="group">ID del grupo.</param>
    [HttpGet("read/on/all")]
    [IdentityToken]
    public async Task<HttpReadAllResponse<GroupModel>> OnMembers([FromHeader] string token, [FromQuery] int organization, [FromQuery] int identity)
    {

        // Token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();

 
        // Confirmar el rol.
        var (_, roles) = await rolesIam.RolesOn(tokenInfo.IdentityId, organization);

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