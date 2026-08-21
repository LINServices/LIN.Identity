namespace LIN.Cloud.Identity.Areas.Organizations;

[IdentityToken]
[Route("[controller]")]
public class IdentityController(IOrganizationMemberRepository directoryMembersData, IIdentityRolesRepository identityRolesData, IIamService rolesIam) : AuthenticationBaseController
{

    /// <summary>
    /// Crear nuevo rol en una identidad.
    /// </summary>
    [HttpPost]
    public async Task<HttpResponseBase> Create([FromBody] IdentityRolesModel rolModel)
    {
        if (rolModel.Rol == Roles.None)
            return new(Responses.InvalidParam);

        var roles = await rolesIam.Validate(UserInformation.IdentityId, rolModel.OrganizationId);

        bool iam = ValidateRoles.ValidateAlterMembers(roles);

        if (!iam)
            return new()
            {
                Message = "No tienes acceso para crear grupos.",
                Response = Responses.Unauthorized
            };

        rolModel.Identity = new()
        {
            Id = rolModel.IdentityId
        };

        rolModel.Organization = new()
        {
            Id = rolModel.OrganizationId
        };

        var response = await identityRolesData.Create(rolModel);

        if (response.Response != Responses.Success)
            return new()
            {
                Response = response.Response
            };

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

        var roles = await rolesIam.Validate(UserInformation.IdentityId, organization);

        bool iam = ValidateRoles.ValidateRead(roles);

        if (!iam)
            return new()
            {
                Message = "No tienes acceso para leer permisos.",
                Response = Responses.Unauthorized
            };

        var isIn = await directoryMembersData.IamIn(identity, organization);

        if (isIn.Response != Responses.Success)
            return new()
            {
                Message = $"La identidad {identity} no pertenece a la organización de contexto.",
                Response = Responses.NotFoundDirectory
            };

        var response = await identityRolesData.ReadAll(identity, organization);

        if (response.Response != Responses.Success)
            return new()
            {
                Response = response.Response
            };

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

        var roles = await rolesIam.Validate(UserInformation.IdentityId, organization);

        bool iam = ValidateRoles.ValidateAlterMembers(roles);

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

        var response = await identityRolesData.Remove(identity, rol, organization);

        if (response.Response != Responses.Success)
            return new()
            {
                Response = response.Response
            };

        return response;

    }

}