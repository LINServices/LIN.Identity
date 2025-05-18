namespace LIN.Cloud.Identity.Services.Interfaces;

internal interface IPolicyOrchestrator
{
    Task<ReadOneResponse<PolicyValidatorContext>> ValidatePoliciesForOrganization(AuthenticationRequest request);
    Task<ReadOneResponse<PolicyValidatorContext>> ValidatePoliciesForApplication(AuthenticationRequest request, int appId);
}