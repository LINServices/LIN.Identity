namespace LIN.Cloud.Identity.Services.Interfaces;

public interface IIamService
{
    Task<IEnumerable<Roles>> IamIdentity(int identity1, int identity2);
    Task<IEnumerable<Roles>> Validate(int identity, int organization);
    Task<IamLevels> IamPolicy(int identity, int policy);
}