namespace LIN.Cloud.Identity.Services.Auth.Interfaces;

public interface IAllowService
{
    Task<bool> IsAllow(IEnumerable<int> identities, int appId);
}