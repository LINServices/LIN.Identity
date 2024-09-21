namespace LIN.Cloud.Identity.Services.Utils;

public interface IIdentityService
{
    Task<List<int>> GetIdentities(int identity);
}