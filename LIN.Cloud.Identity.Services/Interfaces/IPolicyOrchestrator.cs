namespace LIN.Cloud.Identity.Services.Interfaces;

internal interface IPolicyOrchestrator
{
    Task<ReadOneResponse<PolicyValidatorContext>> ValidatePoliciesForOrganization(AuthenticationRequest request);
}