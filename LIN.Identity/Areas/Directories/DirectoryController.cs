namespace LIN.Identity.Areas.Directories;


[Route("directory")]
public class DirectoryController : ControllerBase
{

    /// <summary>
    /// Obtener un directorio.
    /// </summary>
    /// <param name="id">ID del directorio.</param>
    /// <param name="token">Token de acceso.</param>
    [HttpGet("read/id")]
    public async Task<HttpReadOneResponse<DirectoryModel>> Read([FromQuery] int id, [FromHeader] string token)
    {

        // Id es invalido.
        if (id <= 0)
            return new(Responses.InvalidParam);

        // Información del token.
        var (isValid, _, user, orgId, _) = Jwt.Validate(token);

        // Token es invalido.
        if (!isValid)
            return new ReadOneResponse<DirectoryModel>()
            {
                Response = Responses.Unauthorized,
                Message = "Token invalido."
            };

        // Obtiene el usuario.
        var response = await Data.Directories.Read(id);

        // Si es erróneo
        if (response.Response != Responses.Success)
            return new ReadOneResponse<DirectoryModel>()
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
    public async Task<HttpReadAllResponse<DirectoryMember>> ReadAll([FromHeader] string token)
    {

        // Información del token.
        var (isValid, _, user, _, _) = Jwt.Validate(token);

        // Token es invalido.
        if (!isValid)
            return new ReadAllResponse<DirectoryMember>()
            {
                Response = Responses.Unauthorized,
                Message = "Token invalido."
            };

        // Obtiene el usuario.
        var response = await Data.DirectoryMembers.ReadAll(user);

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