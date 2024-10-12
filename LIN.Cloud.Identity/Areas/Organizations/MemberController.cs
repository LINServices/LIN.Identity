using LIN.Types.Cloud.Identity.Abstracts;

namespace LIN.Cloud.Identity.Areas.Organizations;

[Route("orgs/members")]
public class MemberController(Data.Organizations organizationsData, Data.Accounts accountsData, Data.DirectoryMembers directoryMembersData, Data.GroupMembers groupMembers, RolesIam rolesIam) : ControllerBase
{

    /// <summary>
    /// Agregar una identidad externa a la organización.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    /// <param name="organization">Id de la organización.</param>
    /// <param name="ids">Lista de ids a agregar.</param>
    /// <returns>Retorna el resultado del proceso.</returns>
    [HttpPost("invite")]
    [IdentityToken]
    public async Task<HttpCreateResponse> Create([FromHeader] string token, [FromQuery] int organization, [FromBody] List<int> ids)
    {

        // Token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();

        // Confirmar el rol.
        var roles = await rolesIam.RolesOn(tokenInfo.IdentityId, organization);

        // Iam.
        bool iam = ValidateRoles.ValidateInviteMembers(roles);

        // Si no tiene permisos.
        if (!iam)
            return new()
            {
                Message = "No tienes autorización para invitar entidades externas en esta organización.",
                Response = Responses.Unauthorized
            };

        // Solo elementos distintos.
        ids = ids.Distinct().ToList();

        // Valida si el usuario pertenece a la organización.
        var (existentes, noUpdated) = await directoryMembersData.IamIn(ids, organization);

        var directoryId = await organizationsData.ReadDirectory(organization);

        // Crear el usuario.
        var response = await groupMembers.Create(noUpdated.Select(id => new GroupMember
        {
            Group = new()
            {
                Id = directoryId.Model,
            },
            Identity = new()
            {
                Id = id
            },
            Type = GroupMemberTypes.Guest
        }));

        response.Message = $"Se agregaron {noUpdated.Count} integrantes como invitados y se omitieron {existentes.Count} debido a que ya pertenecen a esta organización.";

        // Retorna el resultado
        return response;

    }


    /// <summary>
    /// Crea un nuevo miembro en una organización.
    /// </summary>
    /// <param name="modelo">Modelo de la cuenta.</param>
    /// <param name="token">Token de acceso de un administrador.</param>
    /// <param name="organization">Id de la organización.</param>
    [HttpPost]
    [IdentityToken]
    public async Task<HttpCreateResponse> Create([FromBody] AccountModel modelo, [FromHeader] string token, [FromHeader] int organization)
    {

        // Validar el modelo.
        if (modelo == null || modelo.Identity == null || string.IsNullOrWhiteSpace(modelo.Identity.Unique) || string.IsNullOrWhiteSpace(modelo.Name))
            return new()
            {
                Response = Responses.InvalidParam,
                Message = "Uno o varios parámetros inválidos."
            };

        // Token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();

        // Ajustar el modelo.
        modelo.Visibility = Visibility.Hidden;
        modelo.Password = $"pwd@{DateTime.Now.Year}";
        modelo = Services.Formats.Account.Process(modelo);

        // Organización.
        var orgIdentity = await organizationsData.GetDomain(organization);

        // Validar.
        if (orgIdentity.Response != Responses.Success)
            return new(Responses.NotRows)
            {
                Message = $"No se encontró la organización con Id '{organization}'"
            };

        // Validar usuario y nombre.
        var (pass, message) = Services.Formats.Account.Validate(modelo);

        // Si no fue valido.
        if (!pass)
            return new(Responses.InvalidParam)
            {
                Message = message
            };

        // Agregar la identidad.
        modelo.Identity.Unique = $"{modelo.Identity.Unique}@{orgIdentity.Model.Unique}";

        // Confirmar el rol.
        var roles = await rolesIam.RolesOn(tokenInfo.IdentityId, organization);

        // Iam.
        bool iam = ValidateRoles.ValidateAlterMembers(roles);

        // Si no tiene permisos.
        if (!iam)
            return new()
            {
                Message = "No tienes acceso para crear cuentas en esta organización.",
                Response = Responses.Unauthorized
            };

        // Creación del usuario
        var response = await accountsData.Create(modelo, organization);

        // Evaluación
        if (response.Response != Responses.Success)
            return new(response.Response);

        // Retorna el resultado
        return new CreateResponse()
        {
            LastID = response.Model.Id,
            Response = Responses.Success,
            Message = "Success"
        };

    }


    /// <summary>
    /// Obtiene la lista de integrantes asociados a una organización.
    /// </summary>
    /// <param name="token">Token de acceso</param>
    [HttpGet]
    [IdentityToken]
    public async Task<HttpReadAllResponse<SessionModel<GroupMember>>> ReadAll([FromHeader] string token, [FromHeader] int organization)
    {

        // Información del token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new(); ;

        // Confirmar el rol.
        var roles = await rolesIam.RolesOn(tokenInfo.IdentityId, organization);

        // Iam.
        bool iam = ValidateRoles.ValidateRead(roles);

        // Si no tiene permisos.
        if (!iam)
            return new()
            {
                Message = "No tienes acceso a la información este directorio.",
                Response = Responses.Unauthorized
            };

        // Obtiene los miembros.
        var members = await directoryMembersData.ReadMembersByOrg(organization);

        // Error al obtener los integrantes.
        if (members.Response != Responses.Success)
            return new ReadAllResponse<SessionModel<GroupMember>>
            {
                Message = "No se encontró la organización.",
                Response = Responses.Unauthorized
            };

        // Retorna el resultado
        return members;

    }


    /// <summary>
    /// Agregar una identidad externa a la organización.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    /// <param name="organization">Id de la organización.</param>
    /// <param name="ids">Lista de ids a agregar.</param>
    /// <returns>Retorna el resultado del proceso.</returns>
    [HttpPost("expulse")]
    [IdentityToken]
    public async Task<HttpResponseBase> Expulse([FromHeader] string token, [FromQuery] int organization, [FromBody] List<int> ids)
    {

        // Token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();

        // Confirmar el rol.
        var roles = await rolesIam.RolesOn(tokenInfo.IdentityId, organization);

        // Iam.
        bool iam = ValidateRoles.ValidateDelete(roles);

        // Si no tiene permisos.
        if (!iam)
            return new()
            {
                Message = "No tienes autorización para eliminar entidades externas en esta organización.",
                Response = Responses.Unauthorized
            };

        // Solo elementos distintos.
        ids = ids.Distinct().ToList();

        // Valida si el usuario pertenece a la organización.
        var (existentes, _) = await directoryMembersData.IamIn(ids, organization);


        var response = await directoryMembersData.Expulse(existentes, organization);

        // Retorna el resultado
        return response;

    }

}