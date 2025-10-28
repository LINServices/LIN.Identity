namespace LIN.Cloud.Identity.Services.Services.Authentication;

internal class GetIdentities(IIdentityService identityService) : IIdentityGetService
{

    /// <summary>
    /// Valida la cuenta de usuario y la contraseña.
    /// </summary>
    public async Task<ResponseBase> Authenticate(AuthenticationRequest request)
    {
        var identityId = request.Account?.IdentityId;

        if (identityId == null)
            return new ResponseBase(Responses.Undefined);

        // Obtener las identidades.
        var identities = await identityService.GetIdentities(identityId.Value);
        request.Identities = identities;
        return new ResponseBase(Responses.Success);
    }

}