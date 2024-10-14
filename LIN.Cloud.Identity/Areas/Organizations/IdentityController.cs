namespace LIN.Cloud.Identity.Areas.Organizations;

[IdentityToken]
[Route("[controller]")]
public class IdentityController(Data.DirectoryMembers directoryMembersData, Data.IdentityRoles identityRolesData, RolesIam rolesIam) : AuthenticationBaseController
{

    /// <summary>
    /// Crear nuevo grupo.
    /// </summary>
    /// <param name="rolModel">Modelo del grupo.</param>
    [HttpPost]
    public async Task<HttpResponseBase> Create([FromBody] IdentityRolesModel rolModel)
    {

        // Confirmar el rol.
        var roles = await rolesIam.RolesOn(AuthenticationInformation.IdentityId, rolModel.OrganizationId);

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
    /// <param name="identity">Identidad</param>
    /// <param name="organization">Id de la organización.</param>
    [HttpGet("roles/all")]
    public async Task<HttpReadAllResponse<IdentityRolesModel>> ReadAll([FromHeader] int identity, [FromHeader] int organization)
    {

        // Confirmar el rol.
        var roles = await rolesIam.RolesOn(AuthenticationInformation.IdentityId, organization);

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
    /// <param name="identity">Identidad</param>
    /// <param name="organization">Id de la organización.</param>
    /// <param name="rol">Rol.</param>
    [HttpDelete("roles")]
    public async Task<HttpResponseBase> ReadAll([FromHeader] int identity, [FromHeader] int organization, [FromHeader] Roles rol)
    {

        // Confirmar el rol.
        var roles = await rolesIam.RolesOn(AuthenticationInformation.IdentityId, organization);

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