namespace LIN.Cloud.Identity.Services.Interfaces;

public interface IPolicyEngine
{
    public Task<bool> IsAuthorized(int identity, int organization);
    public Task<bool> IsAuthorizedForService(int identity, int serviceId);
}