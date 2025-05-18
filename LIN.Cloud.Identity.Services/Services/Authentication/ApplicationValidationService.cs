namespace LIN.Cloud.Identity.Services.Services.Authentication;

internal class ApplicationValidationService(IApplicationRepository applicationRepository, IPolicyOrchestrator policyOrchestrator) : IApplicationValidationService
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

        // Validar políticas.
        var response = await policyOrchestrator.ValidatePoliciesForApplication(request, applicationResponse.Model.Id);

        if (response.Response != Responses.Success)
            return response;

        // Correcto.
        request.ApplicationModel = applicationResponse.Model;
        return new ResponseBase(Responses.Success);
    }


}