namespace LIN.Cloud.Identity.Areas.Directories;


[Route("[controller]")]
public class DirectoryController (Data.DirectoryMembers directoryMembersData, Data.Groups groupsData, RolesIam rolesIam) : ControllerBase
{


    /// <summary>
    /// Obtener los directorios asociados.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    [HttpGet("read/all")]
    [IdentityToken]
    public async Task<HttpReadAllResponse<GroupMember>> ReadAll([FromHeader] string token, [FromHeader] int organization)
    {

        // Token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();

        // Validar organización.
        if (organization <= 0)
            return new()
            {
                Response = Responses.InvalidParam,
                Message = "Id de la organización invalido."
            };

        // Obtiene el usuario.
        var response = await directoryMembersData.Read(tokenInfo.IdentityId, organization);

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
    /// Obtener los integrantes asociados a un directorio.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    /// <param name="directory">ID del directorio.</param>
    [HttpGet("read/members")]
    [IdentityToken]
    public async Task<HttpReadAllResponse<GroupMember>> ReadMembers([FromHeader] string token, [FromQuery] int directory)
    {

        // Token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();

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