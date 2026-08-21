namespace LIN.Cloud.Identity.Services.Services.Authentication;

internal class GetIdentities(IIdentityService identityService) : IIdentityGetService
{

    /// <summary>
    /// Obtiene las identidades asociadas a la cuenta autenticada.
    /// </summary>
    public async Task<ResponseBase> Authenticate(AuthenticationRequest request)
    {
        var identityId = request.Account?.IdentityId;

        if (identityId == null)
            return new ResponseBase(Responses.Undefined);

        var identities = await identityService.GetIdentities(identityId.Value);
        request.Identities = identities;
        return new ResponseBase(Responses.Success);
    }

}