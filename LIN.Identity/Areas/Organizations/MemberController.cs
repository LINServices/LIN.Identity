using LIN.Identity.Validations;

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
    public async Task<HttpCreateResponse> Create([FromBody] AccountModel modelo, [FromHeader] string token, [FromHeader] OrgRoles rol)
    {

        // Validación del modelo.
        if (modelo == null || !modelo.Usuario.Trim().Any() || !modelo.Nombre.Trim().Any())
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

        // Validación del token
        var (isValid, _, userID, _, _) = Jwt.Validate(token);

        // Token es invalido
        if (!isValid)
        {
            return new CreateResponse
            {
                Message = "Token invalido.",
                Response = Responses.Unauthorized
            };
        }


        // Obtiene el usuario
        var userContext = await Data.Accounts.ReadBasic(userID);

        // Error al encontrar el usuario
        if (userContext.Response != Responses.Success)
        {
            return new CreateResponse
            {
                Message = "No se encontró un usuario valido.",
                Response = Responses.Unauthorized
            };
        }

        // Si el usuario no tiene una organización
        if (userContext.Model.OrganizationAccess == null)
        {
            return new CreateResponse
            {
                Message = $"El usuario '{userContext.Model.Usuario}' no pertenece a una organización.",
                Response = Responses.Unauthorized
            };
        }

        // Verificación del rol dentro de la organización
        if (!userContext.Model.OrganizationAccess.Rol.IsAdmin())
        {
            return new CreateResponse
            {
                Message = $"El usuario '{userContext.Model.Usuario}' no puede crear nuevos usuarios en esta organización.",
                Response = Responses.Unauthorized
            };
        }


        // Verificación del rol dentro de la organización
        if (userContext.Model.OrganizationAccess.Rol.IsGretter(rol))
        {
            return new CreateResponse
            {
                Message = $"El '{userContext.Model.Usuario}' no puede crear nuevos usuarios con mas privilegios de los propios.",
                Response = Responses.Unauthorized
            };
        }


        // ID de la organización
        var org = userContext.Model.OrganizationAccess.Organization.ID;


        // Conexión
        var (context, connectionKey) = Conexión.GetOneConnection();

        // Creación del usuario
        var response = await Data.Organizations.Members.Create(modelo, org, rol, context);

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
        var (isValid, _, _, orgID, _) = Jwt.Validate(token);

        // Si el token es invalido.
        if (!isValid)
            return new ReadAllResponse<AccountModel>
            {
                Message = "El token es invalido.",
                Response = Responses.Unauthorized
            };
        
        // Obtiene los miembros.
        var members = await Data.Organizations.Members.ReadAll(orgID);

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