namespace LIN.Cloud.Identity.Services.Auth;

public partial class Authentication
{

    /// <summary>
    /// Modelo de la aplicación.
    /// </summary>
    public ApplicationModel? Application { get; set; } = null;


    /// <summary>
    /// Validar aplicación.
    /// </summary>
    private async Task<bool> ValidateApp()
    {

        // Obtener la restrictions.
        var appResponse = await applications.Read(AppCode);
        Application = appResponse.Model;

        // Si no se requiere validar la restrictions.
        if (!Settings.ValidateApp)
            return true;

        // Validar si la restrictions existe.
        if (appResponse.Response != Responses.Success)
            return false;

        // Respuesta.
        return true;
    }



}