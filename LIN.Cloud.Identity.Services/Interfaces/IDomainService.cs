namespace LIN.Cloud.Identity.Services.Interfaces;

public interface IDomainService
{
    Task<bool> VerifyDns(string domain, string code);
    bool VerifyDomain(string dominio);
}