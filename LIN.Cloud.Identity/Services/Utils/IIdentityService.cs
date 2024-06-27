namespace LIN.Cloud.Identity.Services.Utils;

public interface IIdentityService
{
    Task<List<int>> GetIdenties(int identity);
}