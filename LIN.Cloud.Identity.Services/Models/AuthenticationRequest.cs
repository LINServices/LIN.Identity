namespace LIN.Cloud.Identity.Services.Models;

public class AuthenticationRequest
{
    public string User { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Application { get; set; } = string.Empty;

    public LIN.Types.Cloud.Identity.Models.Identities.AccountModel? Account { get; internal set; }
    public LIN.Types.Cloud.Identity.Models.Identities.ApplicationModel? ApplicationModel { get; internal set; }

}