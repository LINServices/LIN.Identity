namespace LIN.Cloud.Identity.Services.Interfaces;

public interface IAuthenticationService
{
    public Task<ResponseBase> Authenticate(AuthenticationRequest request);
}