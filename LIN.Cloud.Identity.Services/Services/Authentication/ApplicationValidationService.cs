using LIN.Types.Cloud.Identity.Models.Identities;

namespace LIN.Cloud.Identity.Services.Services.Authentication;

internal class ApplicationValidationService(IApplicationRepository applicationRepository, IPolicyOrchestrator policyOrchestrator) : IApplicationValidationService
{

    /// <summary>
    /// Valida la aplicación y sus políticas de acceso.
    /// </summary>
    public async Task<ResponseBase> Authenticate(AuthenticationRequest request)
    {
        ReadOneResponse<ApplicationModel> applicationResponse;

        if (request.ApplicationId <= 0)
            applicationResponse = await applicationRepository.Read(request.Application);
        else
            applicationResponse = await applicationRepository.Read(request.ApplicationId);

        if (applicationResponse.Response != Responses.Success)
            return new ResponseBase(Responses.UnauthorizedByApp)
            {
                Message = "Application not found.",
            };

        var response = await policyOrchestrator.ValidatePoliciesForApplication(request, applicationResponse.Model.Id);

        if (response.Response != Responses.Success)
            return response;

        request.ApplicationModel = applicationResponse.Model;
        return new ResponseBase(Responses.Success);
    }


}