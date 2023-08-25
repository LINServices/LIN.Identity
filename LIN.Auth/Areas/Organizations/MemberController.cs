namespace LIN.Auth.Areas.Organizations;


[Route("orgs/members")]
public class MemberController : ControllerBase
{






























    /// <summary>
    /// Crea una cuenta en una organización
    /// </summary>
    /// <param name="modelo">Modelo del usuario</param>
    [HttpPost("create")]
    public async Task<HttpCreateResponse> Create([FromBody] AccountModel modelo, [FromHeader] string token, [FromHeader] OrgRoles rol)
    {

        // Validación del modelo.
        if (modelo == null || !modelo.Usuario.Trim().Any() || !modelo.Nombre.Trim().Any())
        {
            return new CreateResponse
            {
                Response = Responses.InvalidParam,
                Message = "Uno o varios parámetros inválidos."
            };
        }

        // Organización del modelo
        modelo.ID = 0;
        modelo.Creación = DateTime.Now;
        modelo.Estado = AccountStatus.Normal;
        modelo.Insignia = AccountBadges.None;
        modelo.Rol = AccountRoles.User;
        modelo.OrganizationAccess = null;
        modelo.Perfil = modelo.Perfil.Length == 0
                               ? System.IO.File.ReadAllBytes("wwwroot/profile.png")
                               : modelo.Perfil;


        // Establece la contraseña default
        string password = $"ChangePwd@{modelo.Creación:dd.MM.yyyy}";

        // Contraseña default
        modelo.Contraseña = EncryptClass.Encrypt(Conexión.SecreteWord + password);

        // Validación del token
        var (isValid, _, userID, _) = Jwt.Validate(token);

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
        var userContext = await Data.Accounts.Read(userID, true, false, true);

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
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

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
    /// 
    /// </summary>
    /// <param name="modelo">Modelo del usuario</param>
    [HttpGet]
    public async Task<HttpReadAllResponse<AccountModel>> Create([FromHeader] string token)
    {

        var (isValid, _, _, orgID) = Jwt.Validate(token);


        if (!isValid)
        {
            return new ReadAllResponse<AccountModel>
            {
                Message = "",
                Response = Responses.Unauthorized
            };
        }

        var members = await Data.Organizations.Members.ReadAll(orgID);


        if (members.Response != Responses.Success)
        {
            return new ReadAllResponse<AccountModel>
            {
                Message = "No found Organization",
                Response = Responses.Unauthorized
            };
        }



        // Conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        context.CloseActions(connectionKey);

        // Retorna el resultado
        return members;

    }






}