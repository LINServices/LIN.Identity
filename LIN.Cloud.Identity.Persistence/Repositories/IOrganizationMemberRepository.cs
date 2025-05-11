namespace LIN.Cloud.Identity.Persistence.Repositories;

public interface IOrganizationMemberRepository
{

    /// <summary>
    /// Obtener organizaciones donde una identidad es integrante.
    /// </summary>
    /// <param name="id">Id de la identidad.</param>
    Task<ReadAllResponse<OrganizationModel>> ReadAll(int id);


    /// <summary>
    /// Validar si una identidad es integrante de una organización.
    /// </summary>
    /// <param name="id">Id de la identidad.</param>
    /// <param name="organization">Id de la organización.</param>
    Task<ReadOneResponse<GroupMemberTypes>> IamIn(int id, int organization);


    /// <summary>
    /// Expulsar a una identidad de una organización.
    /// </summary>
    /// <param name="ids">Lista de identidades.</param>
    /// <param name="organization">Id de la organización.</param>
    Task<ResponseBase> Expulse(IEnumerable<int> ids, int organization);

}