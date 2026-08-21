using LIN.Types.Cloud.Identity.Abstracts;

namespace LIN.Cloud.Identity.Areas.Organizations;

[IdentityToken]
[Route("orgs/members")]
public class OrganizationMembersController(IOrganizationRepository organizationsData, IAccountRepository accountsData, IOrganizationMemberRepository directoryMembersData, IGroupMemberRepository groupMembers, IIamService rolesIam) : AuthenticationBaseController
{

    /// <summary>
    /// Agregar identidades externas a la organización.
    /// </summary>
    /// <param name="organization">Id de la organización.</param>
    /// <param name="ids">Lista de ids a agregar.</param>
    /// <returns>Retorna el resultado del proceso.</returns>
    [HttpPost("invite")]
    public async Task<HttpCreateResponse> AddExternalMembers([FromQuery] int organization, [FromBody] IEnumerable<int> ids)
    {
        var roles = await rolesIam.Validate(UserInformation.IdentityId, organization);

        bool iam = ValidateRoles.ValidateInviteMembers(roles);

        if (!iam)
            return new()
            {
                Message = "No tienes autorización para invitar entidades externas en esta organización.",
                Response = Responses.Unauthorized
            };

        ids = ids.Distinct();

        // Valida si la identidad pertenece a la organización.
        var (existentes, noUpdated) = await directoryMembersData.IamIn(ids, organization);

        var directoryId = await organizationsData.ReadDirectory(organization);

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

        response.Message = $"Se agregaron {noUpdated.Count} integrantes como invitados y se omitieron {existentes.Count()} debido a que ya pertenecen a esta organización.";

        return response;
    }


    /// <summary>
    /// Crea un nuevo miembro en una organización.
    /// </summary>
    /// <param name="modelo">Modelo de la cuenta.</param>
    /// <param name="organization">Id de la organización.</param>
    [HttpPost]
    public async Task<HttpCreateResponse> Create([FromBody] AccountModel modelo, [FromHeader] int organization)
    {

        // Validar el modelo.
        if (modelo == null || modelo.Identity == null || string.IsNullOrWhiteSpace(modelo.Identity.Unique) || string.IsNullOrWhiteSpace(modelo.Name))
            return new()
            {
                Response = Responses.InvalidParam,
                Message = "Uno o varios parámetros inválidos."
            };

        // Ajustar el modelo.
        modelo.Visibility = Visibility.Hidden;
        modelo.Password = $"pwd@{DateTime.UtcNow.Year}";
        modelo = Services.Formats.Account.Process(modelo);
        modelo.AccountType = AccountTypes.Work;

        var orgIdentity = await organizationsData.GetDomain(organization);

        if (orgIdentity.Response != Responses.Success)
            return new(Responses.NotRows)
            {
                Message = $"No se encontró la organización con Id '{organization}'"
            };

        // Validar usuario y nombre.
        var errors = Services.Formats.Account.Validate(modelo);

        if (errors.Count > 0)
            return new(Responses.InvalidParam)
            {
                Message = "Error al crear la cuenta",
                Errors = errors
            };

        // Componer el identificador único con el dominio de la organización.
        modelo.Identity.Unique = $"{modelo.Identity.Unique}@{orgIdentity.Model.Unique}";

        var roles = await rolesIam.Validate(UserInformation.IdentityId, organization);

        bool iam = ValidateRoles.ValidateAlterMembers(roles);

        if (!iam)
            return new()
            {
                Message = "No tienes acceso para crear nuevos usuarios en esta organización.",
                Response = Responses.Unauthorized
            };

        var response = await accountsData.Create(modelo, organization);

        if (response.Response != Responses.Success)
            return new(response.Response);

        return new CreateResponse()
        {
            LastId = response.Model.Id,
            Response = Responses.Success,
            Message = "Success"
        };

    }


    /// <summary>
    /// Obtiene la lista de integrantes asociados a una organización.
    /// </summary>
    [HttpGet("accounts")]
    public async Task<HttpReadAllResponse<SessionModel<GroupMember>>> ReadAll([FromHeader] int organization)
    {

        var roles = await rolesIam.Validate(UserInformation.IdentityId, organization);

        bool iam = ValidateRoles.ValidateRead(roles);

        if (!iam)
            return new()
            {
                Message = "No tienes acceso a la información este directorio.",
                Response = Responses.Unauthorized
            };

        var members = await directoryMembersData.ReadUserAccounts(organization);

        if (members.Response != Responses.Success)
            return new ReadAllResponse<SessionModel<GroupMember>>
            {
                Message = "No se encontró la organización.",
                Response = Responses.Unauthorized
            };

        return members;
    }


    /// <summary>
    /// Expulsar identidades externas de la organización.
    /// </summary>
    /// <param name="organization">Id de la organización.</param>
    /// <param name="ids">Lista de ids a expulsar.</param>
    /// <returns>Retorna el resultado del proceso.</returns>
    [HttpPost("expulse")]
    public async Task<HttpResponseBase> Expulse([FromQuery] int organization, [FromBody] IEnumerable<int> ids)
    {

        var roles = await rolesIam.Validate(UserInformation.IdentityId, organization);

        bool iam = ValidateRoles.ValidateDelete(roles);

        if (!iam)
            return new()
            {
                Message = "No tienes autorización para eliminar entidades externas en esta organización.",
                Response = Responses.Unauthorized
            };

        ids = ids.Distinct();

        // Valida si la identidad pertenece a la organización.
        var (existentes, _) = await directoryMembersData.IamIn(ids, organization);

        var response = await directoryMembersData.Expulse(existentes, organization);
        return response;
    }

}