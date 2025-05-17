using LIN.Cloud.Identity.Services.Models;

namespace LIN.Cloud.Identity.Services.Services.Authentication;

internal class ApplicationValidationService(IApplicationRepository applicationRepository) : IApplicationValidationService
{

    /// <summary>
    /// Valida la cuenta de usuario y la contraseña.
    /// </summary>
    public async Task<ResponseBase> Authenticate(AuthenticationRequest request)
    {
        // Obtener la aplicación.
        var applicationResponse = await applicationRepository.Read(request.Application);

        if (applicationResponse.Response != Responses.Success)
            return new ResponseBase(Responses.UnauthorizedByApp)
            {
                Message = "Application not found.",
            };

        // Validar politicas de IP.

        // Validar politicas de tiempo.

        // Validar politicas de identidad.

        // Correcto.
        request.ApplicationModel = applicationResponse.Model;
        return new ResponseBase(Responses.Success);
    }


}