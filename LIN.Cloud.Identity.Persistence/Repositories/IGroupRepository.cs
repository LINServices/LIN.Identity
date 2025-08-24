namespace LIN.Cloud.Identity.Persistence.Repositories;

public interface IGroupRepository
{

    /// <summary>
    /// Crear nuevo grupo.
    /// </summary>
    Task<ReadOneResponse<GroupModel>> Create(GroupModel modelo);

    /// <summary>
    /// Obtener un grupo según el Id.
    /// </summary>
    /// <param name="id">Id del grupo.</param>
    Task<ReadOneResponse<GroupModel>> Read(int id);

    /// <summary>
    /// Obtener un grupo según el Id de la identidad.
    /// </summary>
    /// <param name="id">Identidad.</param>
    Task<ReadOneResponse<GroupModel>> ReadByIdentity(int id);

    /// <summary>
    /// Obtener los grupos asociados a una organización.
    /// </summary>
    /// <param name="organization">Organización.</param>
    Task<ReadAllResponse<GroupModel>> ReadAll(int organization);

    /// <summary>
    /// Obtener la organización propietaria de un grupo.
    /// </summary>
    /// <param name="id">Id del grupo.</param>
    Task<ReadOneResponse<int>> GetOwner(int id);

    /// <summary>
    /// Obtener la organización propietaria de un grupo.
    /// </summary>
    /// <param name="id">Id de la identidad del grupo.</param>
    Task<ReadOneResponse<int>> GetOwnerByIdentity(int id);

}