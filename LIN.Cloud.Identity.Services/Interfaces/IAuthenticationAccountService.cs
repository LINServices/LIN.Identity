using LIN.Types.Cloud.Identity.Models.Identities;

namespace LIN.Cloud.Identity.Services.Interfaces;

public interface IAuthenticationAccountService : IAuthenticationService
{
    public AccountModel? Account { get; }
    public string GenerateToken();
}
