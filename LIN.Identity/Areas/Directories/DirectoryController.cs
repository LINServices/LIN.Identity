using LIN.Identity.Data.Areas.Directories;

namespace LIN.Identity.Areas.Directories;


[Route("directory")]
public class DirectoryController : ControllerBase
{


    /// <summary>
    /// Obtener un directorio.
    /// </summary>
    /// <param name="id">ID del directorio.</param>
    /// <param name="findIdentity">Identidad de contexto.</param>
    /// <param name="token">Token de acceso.</param>
    [HttpGet("read/id")]
    [TokenAuth]
    public async Task<HttpReadOneResponse<DirectoryMember>> Read([FromQuery] int id, [FromQuery] int findIdentity, [FromHeader] string token)
    {

        // Id es invalido.
        if (id <= 0)
            return new(Responses.InvalidParam);

        // Token.
        JwtModel tokenInfo = HttpContext.Items["token"] as JwtModel ?? new();

       


        // Acceso IAM.
        var (_, _, roles) = await Data.Queries.Directories.Get(tokenInfo.IdentityId);

        // Si no hay acceso.
        if (Roles.View(roles))
            return new ReadOneResponse<DirectoryMember>()
            {
                Response = Responses.Unauthorized,
                Message = "No tienes permisos para acceder a este recurso."
            };

        // Obtiene el directorio.
        var response = await Data.Areas.Directories.Directories.Read(id, findIdentity);

        // Si es erróneo
        if (response.Response != Responses.Success)
            return new ReadOneResponse<DirectoryMember>()
            {
                Response = response.Response
            };

        // Retorna el resultado
        return response;

    }



    /// <summary>
    /// Obtener los directorios asociados.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    [HttpGet("read/all")]
    [TokenAuth]
    public async Task<HttpReadAllResponse<DirectoryMember>> ReadAll([FromHeader] string token)
    {

        // Token.
        JwtModel tokenInfo = HttpContext.Items["token"] as JwtModel ?? new();

        // Si el token no es valido.
      

        // Obtiene el usuario.
        var response = await Data.Areas.Directories.Directories.ReadAll(tokenInfo.IdentityId);

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
    [TokenAuth]
    public async Task<HttpReadAllResponse<DirectoryMember>> ReadAll([FromHeader] string token, [FromQuery] int directory)
    {

        // Token.
        JwtModel tokenInfo = HttpContext.Items["token"] as JwtModel ?? new();

   


        // Acceso IAM.
        var (_, _, roles) = await Data.Queries.Directories.Get(tokenInfo.IdentityId);

        // Si no hay acceso.
        if (Roles.ViewMembers(roles))
            return new()
            {
                Response = Responses.Unauthorized,
                Message = "No tienes permisos para visualizar los integrantes de este recurso."
            };

        // Obtiene el usuario.
        var response = await DirectoryMembers.ReadMembers(directory);

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