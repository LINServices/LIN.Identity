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
    public async Task<HttpReadOneResponse<DirectoryMember>> Read([FromQuery] int id, [FromQuery] int findIdentity, [FromHeader] string token)
    {

        // Id es invalido.
        if (id <= 0)
            return new(Responses.InvalidParam);

        // Información del token.
        var (isValid, _, _, _, _, identity) = Jwt.Validate(token);;

        // Token es invalido.
        if (!isValid)
            return new ReadOneResponse<DirectoryMember>()
            {
                Response = Responses.Unauthorized,
                Message = "Token invalido."
            };


        // Acceso IAM.
        var iam = await Services.Iam.Directories.ValidateAccess(identity, id);

        // Validar Iam.
        if (iam.Model == IamLevels.NotAccess)
            return new ReadOneResponse<DirectoryMember>()
            {
                Message = "No tienes acceso a este directorio, si crees que es un error comunícate con tu administrador.",
                Response = Responses.Unauthorized
            };

        // Obtiene el usuario.
        var response = await Data.Directories.Read(id, findIdentity);

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
    public async Task<HttpReadAllResponse<DirectoryMember>> ReadAll([FromHeader] string token)
    {

        // Información del token.
        var (isValid, _, user, _, _, _) = Jwt.Validate(token);;

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



    /// <summary>
    /// Obtener los integrantes asociados a un directorio.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    /// <param name="directory">ID del directorio.</param>
    [HttpGet("read/members")]
    public async Task<HttpReadAllResponse<DirectoryMember>> ReadAll([FromHeader] string token, [FromQuery] int directory)
    {

        // Información del token.
        var (isValid, _, user, _, _, identity) = Jwt.Validate(token); ;

        // Token es invalido.
        if (!isValid)
            return new ReadAllResponse<DirectoryMember>()
            {
                Response = Responses.Unauthorized,
                Message = "Token invalido."
            };


        // Acceso IAM.
        var iam = await Services.Iam.Directories.ValidateAccess(identity, directory);

        // Roles disponibles.
        IEnumerable<IamLevels> have = [IamLevels.Privileged, IamLevels.Visualizer];

        // No tiene un permitido.
        if (!have.Contains(iam.Model))
            return new()
            {
                Message = "No tienes acceso a este recurso.",
                Response = Responses.Unauthorized
            };

        // Obtiene el usuario.
        var response = await Data.DirectoryMembers.ReadMembers(directory);

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