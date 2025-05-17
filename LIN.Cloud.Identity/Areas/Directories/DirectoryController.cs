namespace LIN.Cloud.Identity.Areas.Directories;

[IdentityToken]
[Route("[controller]")]
public class DirectoryController(IOrganizationMemberRepository directoryMembersData, IGroupRepository groupsData, IIamService rolesIam) : AuthenticationBaseController
{

    /// <summary>
    /// Obtener los integrantes del directorio general de la organización.
    /// </summary>
    /// <param name="organization">Id de la organización.</param>
    /// <returns>Retorna la lista de integrantes./returns>
    [HttpGet("read/all")]
    public async Task<HttpReadAllResponse<OrganizationModel>> ReadAll([FromHeader] int organization)
    {
        // Validar organización.
        if (organization <= 0)
            return new()
            {
                Response = Responses.InvalidParam,
                Message = "Id de la organización invalido."
            };

        // Obtiene el usuario.
        var response = await directoryMembersData.ReadAll(
            organization);

        // Si es erróneo.
        if (response.Response != Responses.Success)
            return new()
            {
                Response = response.Response
            };

        // Retorna el resultado.
        return response;
    }


    /// <summary>
    /// Obtener los integrantes de un grupo.
    /// </summary>
    /// <param name="directory">Id del directorio.</param>
    /// <returns>Retorna la lista de integrantes del directorio.</returns>
    [HttpGet("read/members")]
    public async Task<HttpReadAllResponse<GroupMember>> ReadMembers([FromQuery] int directory)
    {

        // Obtener la organización.
        var orgId = await groupsData.GetOwner(directory);

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

        //// Obtiene el usuario.
        //var response = await directoryMembersData.ReadMembers(directory);

        //// Si es erróneo
        //if (response.Response != Responses.Success)
        //    return new()
        //    {
        //        Response = response.Response
        //    };

        //// Retorna el resultado
        //return response;
        return new()
        {
            Response = Responses.Success
        };

    }

}