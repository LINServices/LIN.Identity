using LIN.Cloud.Identity.Services.Models;

namespace LIN.Cloud.Identity.Services.Interfaces;

internal interface IPolicyOrchestrator
{
    Task<ReadOneResponse<PolicyValidatorContext>> ValidatePoliciesForOrganization(AuthenticationRequest request);
}