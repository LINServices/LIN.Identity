namespace LIN.Cloud.Identity.Areas.Organizations;


[Route("[controller]")]
public class IdentityController(Data.DirectoryMembers directoryMembersData, Data.IdentityRoles identityRolesData, RolesIam rolesIam) : ControllerBase
{


    /// <summary>
    /// Crear nuevo grupo.
    /// </summary>
    /// <param name="rolModel">Modelo del grupo.</param>
    /// <param name="token">Token de acceso.</param>
    [HttpPost]
    [IdentityToken]
    public async Task<HttpResponseBase> Create([FromBody] IdentityRolesModel rolModel, [FromHeader] string token)
    {

        // Token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();

        // Confirmar el rol.
        var (_, roles) = await rolesIam.RolesOn(tokenInfo.IdentityId, rolModel.OrganizationId);

        // Iam.
        bool iam = ValidateRoles.ValidateAlterMembers(roles);

        // Si no tiene permisos.
        if (!iam)
            return new()
            {
                Message = "No tienes acceso para crear grupos.",
                Response = Responses.Unauthorized
            };

        // Identidad.
        rolModel.Identity = new()
        {
            Id = rolModel.IdentityId
        };

        // Organización.
        rolModel.Organization = new()
        {
            Id = rolModel.OrganizationId
        };

        // Obtener el modelo.
        var response = await identityRolesData.Create(rolModel);

        // Si es erróneo
        if (response.Response != Responses.Success)
            return new()
            {
                Response = response.Response
            };

        // Retorna el resultado
        return new()
        {
            Response = Responses.Success
        };

    }



    /// <summary>
    /// Obtener los roles asociados a una identidad.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    /// <param name="identity">Identidad</param>
    /// <param name="organization">Id de la organización.</param>
    [HttpGet("roles/all")]
    [IdentityToken]
    public async Task<HttpReadAllResponse<IdentityRolesModel>> ReadAll([FromHeader] string token, [FromHeader] int identity, [FromHeader] int organization)
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
                Message = "No tienes acceso para leer grupos.",
                Response = Responses.Unauthorized
            };



        var isIn = await directoryMembersData.IamIn(identity, organization);


        if (isIn.Response != Responses.Success)
            return new()
            {
                Message = $"La identidad {identity} no pertenece a la organización de contexto.",
                Response = Responses.NotFoundDirectory
            };


        // Obtener el modelo.
        var response = await identityRolesData.ReadAll(identity, organization);

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
    /// Eliminar los roles asociados a una identidad.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    /// <param name="identity">Identidad</param>
    /// <param name="organization">Id de la organización.</param>
    /// <param name="rol">Rol.</param>
    [HttpDelete("roles")]
    [IdentityToken]
    public async Task<HttpResponseBase> ReadAll([FromHeader] string token, [FromHeader] int identity, [FromHeader] int organization, [FromHeader] Roles rol)
    {

        // Token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();

        // Confirmar el rol.
        var (_, roles) = await rolesIam.RolesOn(tokenInfo.IdentityId, organization);

        // Iam.
        bool iam = ValidateRoles.ValidateAlterMembers(roles);

        // Si no tiene permisos.
        if (!iam)
            return new()
            {
                Message = "No tienes acceso para leer grupos.",
                Response = Responses.Unauthorized
            };



        var isIn = await directoryMembersData.IamIn(identity, organization);


        if (isIn.Response != Responses.Success)
            return new()
            {
                Message = $"La identidad {identity} no pertenece a la organización de contexto.",
                Response = Responses.NotFoundDirectory
            };


        // Obtener el modelo.
        var response = await identityRolesData.Remove(identity, rol, organization);

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