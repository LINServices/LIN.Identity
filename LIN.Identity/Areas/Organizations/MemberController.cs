using LIN.Identity.Data.Areas.Organizations;

namespace LIN.Identity.Areas.Organizations;


[Route("orgs/members")]
public class MemberController : ControllerBase
{


    /// <summary>
    /// Crea un nuevo miembro en una organización.
    /// </summary>
    /// <param name="modelo">Modelo de la cuenta</param>
    /// <param name="token">Token de acceso de un administrador</param>
    /// <param name="rol">Rol asignado</param>
    [HttpPost]
    public async Task<HttpCreateResponse> Create([FromBody] AccountModel modelo, [FromHeader] string token, [FromHeader] DirectoryRoles rol)
    {

        // Validación del modelo.
        if (modelo == null || !modelo.Identity.Unique.Trim().Any() || !modelo.Nombre.Trim().Any())
            return new CreateResponse
            {
                Response = Responses.InvalidParam,
                Message = "Uno o varios parámetros inválidos."
            };


        // Visibilidad oculta
        modelo.Visibilidad = AccountVisibility.Hidden;

        // Organización del modelo
        modelo = Account.Process(modelo);

        // Establece la contraseña default
        var password = $"ChangePwd@{modelo.Creación:dd.MM.yyyy}";

        // Contraseña default
        modelo.Contraseña = EncryptClass.Encrypt(password);

        // Token.
        var tokenInfo = Jwt.Validate(token);

        // Si el token no es valido.
        if (!tokenInfo.IsAuthenticated)
            return new()
            {
                Response = Responses.Unauthorized,
                Message = "Token invalido."
            };

        // Obtiene el usuario
        var userContext = await Data.Accounts.ReadBasic(tokenInfo.AccountId);

        // Error al encontrar el usuario
        if (userContext.Response != Responses.Success)
        {
            return new CreateResponse
            {
                Message = "No se encontró un usuario valido.",
                Response = Responses.Unauthorized
            };
        }


        var orgBase = await Data.Areas.Organizations.Organizations.FindBaseDirectory(userContext.Model.IdentityId);

        if (orgBase.Response != Responses.Success)
        {
            return new()
            {
                Response = Responses.NotRows,
                Message = "No se encontró una organización permitida a este usuario."
            };
        }

        // Validar acceso en el directorio con IAM.
        //var iam = await Services.Iam.Directories.ValidateAccess(userContext.Model.IdentityId, orgBase.Model.DirectoryId);


        //DirectoryRoles[] roles = [DirectoryRoles.System, DirectoryRoles.SuperManager, DirectoryRoles.Manager, DirectoryRoles.AccountsOperator, DirectoryRoles.Operator];


        //if (!roles.Contains(iam.Model))
        //    return new CreateResponse
        //    {
        //        Message = $"No tienes permisos suficientes para crear nuevas cuentas en el directorio general de tu organización.",
        //        Response = Responses.Unauthorized
        //    };

        // Conexión
        var (context, connectionKey) = Conexión.GetOneConnection();

        // Creación del usuario
        var response = await Members.Create(modelo, orgBase.Model.DirectoryId, rol, context);

        // Evaluación
        if (response.Response != Responses.Success)
            return new(response.Response);

        // Cierra la conexión
        context.CloseActions(connectionKey);

        // Retorna el resultado
        return new CreateResponse()
        {
            LastID = response.Model.ID,
            Response = Responses.Success,
            Message = "Success"
        };

    }



    /// <summary>
    /// Obtiene la lista de integrantes asociados a una organización.
    /// </summary>
    /// <param name="token">Token de acceso</param>
    [HttpGet]
    public async Task<HttpReadAllResponse<AccountModel>> ReadAll([FromHeader] string token)
    {

        // Información del token.
        var tokenInfo = Jwt.Validate(token); ;

        // Si el token es invalido.
        if (!tokenInfo.IsAuthenticated)
            return new ReadAllResponse<AccountModel>
            {
                Message = "El token es invalido.",
                Response = Responses.Unauthorized
            };

        // Obtiene los miembros.
        var members = await Members.ReadAll(tokenInfo.OrganizationId);

        // Error al obtener los integrantes.
        if (members.Response != Responses.Success)
            return new ReadAllResponse<AccountModel>
            {
                Message = "No se encontró la organización.",
                Response = Responses.Unauthorized
            };

        // Retorna el resultado
        return members;

    }



}