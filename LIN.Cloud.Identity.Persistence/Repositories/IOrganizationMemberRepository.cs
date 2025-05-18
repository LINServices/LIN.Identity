using LIN.Types.Cloud.Identity.Abstracts;

namespace LIN.Cloud.Identity.Persistence.Repositories;

public interface IOrganizationMemberRepository
{

    /// <summary>
    /// Obtener organizaciones donde una identidad es integrante.
    /// </summary>
    /// <param name="id">Id de la identidad.</param>
    Task<ReadAllResponse<GroupMember>> ReadAll(int id);


    /// <summary>
    /// Validar si una identidad es integrante de una organización.
    /// </summary>
    /// <param name="id">Id de la identidad.</param>
    /// <param name="organization">Id de la organización.</param>
    Task<ReadOneResponse<GroupMemberTypes>> IamIn(int id, int organization);


    /// <summary>
    /// Validar si una identidad es integrante de una organización.
    /// </summary>
    /// <param name="id">Id de la identidad.</param>
    /// <param name="organization">Id de la organización.</param>
    Task<(IEnumerable<int> success, List<int> failure)> IamIn(IEnumerable<int> id, int organization);


    /// <summary>
    /// Expulsar a una identidad de una organización.
    /// </summary>
    /// <param name="ids">Lista de identidades.</param>
    /// <param name="organization">Id de la organización.</param>
    Task<ResponseBase> Expulse(IEnumerable<int> ids, int organization);


    /// <summary>
    /// Obtener las organizaciones donde una identidad es integrante.
    /// </summary>
    Task<ReadAllResponse<OrganizationModel>> ReadAllMembers(int identity);


    /// <summary>
    /// Obtener las cuentas de usuario de una organización.
    /// </summary>
    /// <param name="id">Id de la organización.</param>
    Task<ReadAllResponse<SessionModel<GroupMember>>> ReadUserAccounts(int id);

}