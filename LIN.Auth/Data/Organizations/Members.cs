namespace LIN.Auth.Data.Organizations;


public class Members
{


    #region Abstracciones


    /// <summary>
    /// Obtiene la lista de integrantes de una organización
    /// </summary>
    /// <param name="id">ID de la organización</param>
    public async static Task<ReadAllResponse<AccountModel>> ReadAll(int id)
    {
        var (context, contextKey) = Conexión.GetOneConnection();

        var res = await ReadAll(id, context);
        context.CloseActions(contextKey);
        return res;
    }


    #endregion




    /// <summary>
    /// Obtiene la lista de integrantes de una organización.
    /// </summary>
    /// <param name="id">ID de la organización</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ReadAllResponse<AccountModel>> ReadAll(int id, Conexión context)
    {

        // Ejecución
        try
        {

            // Organización
            var org = from O in context.DataBase.OrganizationAccess
                      where O.Organization.ID == id
                      select new AccountModel
                      {
                          Creación = O.Member.Creación,
                          ID = O.Member.ID,
                          Nombre = O.Member.Nombre,
                          Genero = O.Member.Genero,
                          Usuario = O.Member.Usuario,
                          OrganizationAccess = new()
                          {
                              Rol = O.Member.OrganizationAccess == null ? OrgRoles.Undefine : O.Member.OrganizationAccess.Rol
                          }
                      };

            var orgList = await org.ToListAsync();

            // Email no existe
            if (org == null)
                return new(Responses.NotRows);

            return new(Responses.Success, orgList);
        }
        catch
        {
        }

        return new();
    }



}