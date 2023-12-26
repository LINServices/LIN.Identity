using LIN.Identity.Data.Queries;

namespace LIN.Identity.Services.Iam;


public static class Applications
{


    /// <summary>
    /// Validar acceso a un recurso de aplicación.
    /// </summary>
    /// <param name="account">ID de la cuenta</param>
    /// <param name="app">ID de la aplicación</param>
    public static async Task<ReadOneResponse<IamLevels>> ValidateAccess(int account, int app)
    {

        // Obtiene el recurso.
        var resource = await Data.Applications.Read(app);

        // Si no existe el recurso.
        if (resource == null)
            return new()
            {
                Message = "No se encontró el recurso.",
                Response = Responses.NotRows,
                Model = IamLevels.NotAccess
            };

        // App publica.
        if (resource.Model.DirectoryId <= 0)
            return new()
            {
                Response = Responses.Success,
                Model = IamLevels.Visualizer
            };
        




        var (context, contextKey) = Conexión.GetOneConnection();


        var identity = (from a in context.DataBase.Accounts
                        where a.ID == account
                        select a.IdentityId).FirstOrDefault();


        var (directories, _,_) = await Directories.Get(identity);


        // Tiene acceso.
        if (directories.Contains(resource.Model.DirectoryId))
        {
            return new()
            {
                Model = IamLevels.Visualizer,
                Response = Responses.Success
            };
        }

        return new()
        {
            Model = IamLevels.NotAccess,
            Response = Responses.Success
        };

    }



}