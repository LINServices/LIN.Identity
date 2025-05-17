using LIN.Cloud.Identity.Services.Models;

namespace LIN.Cloud.Identity.Services.Interfaces;

public interface IAuthenticationService
{
    public Task<ResponseBase> Authenticate(AuthenticationRequest request);
}