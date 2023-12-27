using LIN.Identity.Data.Areas.Organizations;
using Account = LIN.Identity.Validations.Account;

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
    [TokenAuth]
    public async Task<HttpCreateResponse> Create([FromBody] AccountModel modelo, [FromHeader] string token, [FromHeader] DirectoryRoles rol)
    {

        // Validar el modelo.
        if (modelo == null || modelo.Identity == null || string.IsNullOrWhiteSpace(modelo.Identity.Unique) || string.IsNullOrWhiteSpace(modelo.Nombre))
            return new()
            {
                Response = Responses.InvalidParam,
                Message = "Uno o varios parámetros inválidos."
            };

        // Token.
        JwtModel tokenInfo = HttpContext.Items["token"] as JwtModel ?? new();

        

        // Ajustar el modelo.
        modelo.Visibilidad = AccountVisibility.Hidden;
        modelo.Contraseña = $"ChangePwd@{DateTime.Now.Year}";
        modelo = Account.Process(modelo);

        // Obtiene el usuario.
        var userContext = await Data.Accounts.ReadBasic(tokenInfo.AccountId);

        // Error al encontrar el usuario
        if (userContext.Response != Responses.Success)
            return new CreateResponse
            {
                Message = "No se encontró un usuario valido.",
                Response = Responses.Unauthorized
            };

        // Encontrar el directorio de la organización.
        var orgBase = await Data.Areas.Organizations.Organizations.FindBaseDirectory(userContext.Model.IdentityId);

        // Si no se encontró el directorio.
        if (orgBase.Response != Responses.Success)
            return new()
            {
                Response = Responses.NotRows,
                Message = "No se encontró una organización permitida a este usuario."
            };
        
        // Permisos para alterar los integrantes.
        var iam = Roles.AlterMembers(orgBase.Model.Rol);

        // No tienes permisos.
        if (!iam)
            return new()
            {
                Response = Responses.Unauthorized,
                Message = "No tienes permisos para modificar los integrantes de esta organización."
            };










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
    [TokenAuth]
    public async Task<HttpReadAllResponse<AccountModel>> ReadAll([FromHeader] string token)
    {

        // Información del token.
        JwtModel tokenInfo = HttpContext.Items["token"] as JwtModel ?? new(); ;

      

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