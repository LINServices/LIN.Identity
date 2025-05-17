namespace LIN.Cloud.Identity.Services.Models;

internal class PolicyValidationResult
{
    public bool IsValid { get; set; } = true;
    public string Reason { get; set; } = string.Empty;
}