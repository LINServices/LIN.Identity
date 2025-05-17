namespace LIN.Cloud.Identity.Services.Models;

internal class PolicyValidatorContext
{
    public AuthenticationRequest AuthenticationRequest { get; set; } = null!;
    public List<string> Reasons { get; set; } = [];
    public Dictionary<string, bool?> Evaluated { get; set; } = [];
}