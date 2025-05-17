namespace LIN.Cloud.Identity.Services.Interfaces;

public interface IIamService
{
    Task<IamLevels> IamIdentity(int identity1, int identity2);
    Task<IEnumerable<Roles>> Validate(int identity, int organization);
}