namespace LIN.Cloud.Identity.Services.Services.Authentication;

internal class OrganizationValidationService(IPolicyOrchestrator policyOrchestrator) : IOrganizationValidationService
{

    /// <summary>
    /// Valida políticas de organización.
    /// </summary>
    public async Task<ResponseBase> Authenticate(AuthenticationRequest request)
    {
        // Validar si existe una organización dueña de la cuenta.
        if (request.Account!.Identity.OwnerId is null || request.Account!.Identity.OwnerId <= 0)
            return new ResponseBase(Responses.Success);

        // Validar políticas.
        var response = await policyOrchestrator.ValidatePoliciesForOrganization(request);

        if (response.Response != Responses.Success)
            return response;

        // Correcto.
        return new ResponseBase(Responses.Success);
    }

}