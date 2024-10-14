namespace LIN.Cloud.Identity.Areas.Directories;

[Route("[controller]")]
public class DirectoryController(Data.DirectoryMembers directoryMembersData, Data.Groups groupsData, RolesIam rolesIam) : AuthenticationBaseController
{

    /// <summary>
    /// Obtener los integrantes del directorio general de la organización.
    /// </summary>
    /// <param name="organization">Id de la organización.</param>
    /// <returns>Retorna la lista de integrantes./returns>
    [HttpGet("read/all")]
    [IdentityToken]
    public async Task<HttpReadAllResponse<GroupMember>> ReadAll([FromHeader] int organization)
    {
        // Validar organización.
        if (organization <= 0)
            return new()
            {
                Response = Responses.InvalidParam,
                Message = "Id de la organización invalido."
            };

        // Obtiene el usuario.
        var response = await directoryMembersData.Read(AuthenticationInformation.IdentityId, organization);

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
    /// Obtener los integrantes de un grupo.
    /// </summary>
    /// <param name="directory">Id del directorio.</param>
    /// <returns>Retorna la lista de integrantes del directorio.</returns>
    [HttpGet("read/members")]
    [IdentityToken]
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
        var roles = await rolesIam.RolesOn(AuthenticationInformation.IdentityId, orgId.Model);

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
        var response = await directoryMembersData.ReadMembers(directory);

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