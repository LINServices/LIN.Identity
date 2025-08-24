namespace LIN.Cloud.Identity.Services.Interfaces;

internal interface IIdentityService
{
    Task<List<int>> GetIdentities(int identity);
    Task<Dictionary<int, List<IdentityLevelModel>>> GetLevel(int identity, int organization);
}