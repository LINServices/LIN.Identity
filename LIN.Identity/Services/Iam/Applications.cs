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

        // Si es admin del recurso / creador
        if (resource.Model.AccountID == account)
            return new()
            {
                Response = Responses.Success,
                Model = IamLevels.Privileged
            };

        // Si es un recurso publico.
        if (resource.Model.AllowAnyAccount)
            return new()
            {
                Response = Responses.Success,
                Model = IamLevels.Visualizer
            };

        // Validar acceso al recurso privado.
        var isAllowed = await Data.Applications.IsAllow(app, account);

        // No es permitido.
        if (!isAllowed.Model)
            return new()
            {
                Response = Responses.Success,
                Model = IamLevels.NotAccess
            };

        // Acceso visualizador.
        return new()
        {
            Response = Responses.Success,
            Model = IamLevels.Visualizer
        };


    }


}