namespace LIN.Identity.Data;


public class Policies
{


    #region Abstracciones



    /// <summary>
    /// Crear nueva política.
    /// </summary>
    /// <param name="data">Modelo.</param>
    public static async Task<CreateResponse> Create(PolicyModel data)
    {
        var (context, contextKey) = Conexión.GetOneConnection();
        var response = await Create(data, context);
        context.CloseActions(contextKey);
        return response;
    }



    /// <summary>
    /// Obtener las políticas.
    /// </summary>
    /// <param name="id">Id de la cuenta.</param>
    public static async Task<ReadAllResponse<PolicyModel>> ReadAll(int id)
    {
        var (context, contextKey) = Conexión.GetOneConnection();

        var res = await ReadAll(id, context);
        context.CloseActions(contextKey);
        return res;
    }



    /// <summary>
    /// Valida el acceso a un permiso de una identidad.
    /// </summary>
    /// <param name="identity">ID de la identidad</param>
    public static async Task<ReadOneResponse<bool>> ValidatePermission(int identity, int policy)
    {
        var (context, contextKey) = Conexión.GetOneConnection();

        var res = await ValidatePermission(identity, policy, context);
        context.CloseActions(contextKey);
        return res;
    }



    #endregion



    /// <summary>
    /// Crear política.
    /// </summary>
    /// <param name="data">Modelo</param>
    /// <param name="context">Contexto de conexión.</param>
    public static async Task<CreateResponse> Create(PolicyModel data, Conexión context)
    {

        // ID.
        data.Id = 0;

        // Ejecución
        try
        {

            // Existe el directorio.
            context.DataBase.Attach(data.Directory);

            // Guardar la información.
            await context.DataBase.Policies.AddAsync(data);

            // Llevar info a la BD.
            context.DataBase.SaveChanges();

            return new(Responses.Success, data.Id);
        }
        catch (Exception ex)
        {

            if ((ex.InnerException?.Message.Contains("Violation of UNIQUE KEY constraint") ?? false) || (ex.InnerException?.Message.Contains("duplicate key") ?? false))
                return new(Responses.Undefined);

        }

        return new();
    }



    /// <summary>
    /// Obtiene las políticas asociadas a un usuario.
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    /// <param name="context">Contexto de conexión</param>
    public static async Task<ReadAllResponse<PolicyModel>> ReadAll(int id, Conexión context)
    {

        // Ejecución
        try
        {

            var policies = await (from directory in context.DataBase.DirectoryMembers
                                  where directory.IdentityId == id
                                  join policie in context.DataBase.Policies
                                  on directory.DirectoryId equals policie.DirectoryId
                                  select policie).ToListAsync();


            return new(Responses.Success, policies);

        }
        catch
        {
        }

        return new();
    }




    /// <summary>
    /// Valida el acceso a un permiso de una identidad.
    /// </summary>
    /// <param name="identity">ID de la identidad</param>
    /// <param name="context">Contexto de conexión</param>
    public static async Task<ReadOneResponse<bool>> ValidatePermission(int identity, int policyId, Conexión context)
    {

        // Ejecución
        try
        {

            var (directories, _) = await Queries.Directories.Get(identity);


            // Consulta.
            var policy = await (from p in context.DataBase.Policies
                                where p.Type == PolicyTypes.Permission
                                where p.Id == policyId
                                select p).FirstOrDefaultAsync();

            // Si no se encontró la política.
            if (policy == null)
                return new()
                {
                    Response = Responses.NotRows,
                    Model = false,
                    Message = "Sin definición."
                };


            // No tiene acceso.
            if (!directories.Contains(policy.DirectoryId))
                return new()
                {
                    Response = Responses.PoliciesNotComplied,
                    Model = false,
                    Message = $$"""No tienes permiso para el recurso {{{policyId}}}."""
                };


            return new(Responses.Success, true);

        }
        catch
        {
        }

        return new();
    }


}