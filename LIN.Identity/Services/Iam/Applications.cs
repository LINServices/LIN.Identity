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

        // Si es admin del recurso / creador.
        if (resource.Model.AccountID == account)
            return new()
            {
                Response = Responses.Success,
                Model = IamLevels.Privileged
            };


        // App publica.
        if (resource.Model.DirectoryId <= 0)
        {
            return new()
            {
                Response = Responses.Success,
                Model = IamLevels.Visualizer
            };
        }


        var (context, contextKey) = Conexión.GetOneConnection();


        var directory = (from m in context.DataBase.DirectoryMembers
                where m.AccountId == account
                && m.DirectoryId == resource.Model.DirectoryId
                select m).FirstOrDefault();


        if (directory == null)
        {
            return new()
            {
                Response = Responses.NotRows,
                Model = IamLevels.NotAccess,
                Message = "No tienes acceso a este recurso/app."
            };
        }


        return new()
        {
            Response = Responses.Success,
            Model = (directory.Rol == DirectoryRoles.Administrator) ? IamLevels.Privileged : IamLevels.Visualizer
        };

    }


}