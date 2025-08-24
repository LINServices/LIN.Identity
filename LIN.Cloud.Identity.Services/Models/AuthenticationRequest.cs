namespace LIN.Cloud.Identity.Services.Models;

public class AuthenticationRequest
{
    public string User { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Application { get; set; } = string.Empty;
    public string ThirdPartyToken { get; set; } = string.Empty;
    public bool StrictService { get; set; } = false;
    public int ApplicationId { get; set; }
    public Types.Cloud.Identity.Enumerations.IdentityService Service { get; set; } = Types.Cloud.Identity.Enumerations.IdentityService.LIN;

    public Types.Cloud.Identity.Models.Identities.AccountModel? Account { get; internal set; }
    public Types.Cloud.Identity.Models.Identities.ApplicationModel? ApplicationModel { get; internal set; }

}