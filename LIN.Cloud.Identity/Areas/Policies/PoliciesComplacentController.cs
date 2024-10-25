namespace LIN.Cloud.Identity.Areas.Policies;

[IdentityToken]
[Route("policies/complacent")]
public class PoliciesComplacentController(Data.Policies policiesData, IamRoles iam, IamPolicy iamPolicy) : AuthenticationBaseController
{


    [HttpGet("applicable")]
    public async Task<HttpResponseBase> Applicants([FromHeader] int identity)
    {

        if (identity != UserInformation.IdentityId)
        {
            var iamResult = await iam.IamIdentity(UserInformation.IdentityId, identity);
            if (iamResult != Types.Enumerations.IamLevels.Privileged)
                return new ResponseBase(Responses.Unauthorized)
                {
                    Message = "No tienes permisos para ver los solicitantes de la política."
                };
        }

        var response = await policiesData.ApplicablePolicies(identity);
        return response;
    }


    /// <summary>
    /// Agregar integrantes a una política.
    /// </summary>
    /// <param name="policy">Id de la política.</param>
    /// <param name="identity">Id de la identidad a agregar.</param>
    [HttpPost]
    public async Task<HttpResponseBase> AddMember([FromQuery] string policy, [FromHeader] int identity)
    {

        // Validar Iam.
        var iamResult = await iamPolicy.Validate(UserInformation.IdentityId, policy);

        if (iamResult != Types.Enumerations.IamLevels.Privileged)
            return new ResponseBase(Responses.Unauthorized) { Message = "No tienes permisos para agregar integrantes a la política." };

        // Agregar integrante
        var response = await policiesData.AddMember(identity, policy);

        return response;
    }


    /// <summary>
    /// Eliminar integrantes a una política.
    /// </summary>
    /// <param name="policy">Id de la política.</param>
    /// <param name="identity">Id de la identidad a agregar.</param>
    [HttpDelete]
    public async Task<HttpResponseBase> DeleteMember([FromQuery] string policy, [FromHeader] int identity)
    {

        // Validar Iam.
        var iamResult = await iamPolicy.Validate(UserInformation.IdentityId, policy);

        if (iamResult != Types.Enumerations.IamLevels.Privileged)
            return new ResponseBase(Responses.Unauthorized) { Message = "No tienes permisos para eliminar integrantes de la política." };

        // Agregar integrante
        var response = await policiesData.RemoveMember(identity, policy);

        return response;
    }


    /// <summary>
    /// Validar si tiene autorización.
    /// </summary>
    /// <param name="policy">Id de la política.</param>
    [HttpGet]
    public async Task<HttpResponseBase> IsAllow([FromQuery] string policy)
    {
        var response = await policiesData.HasFor(UserInformation.IdentityId, policy);
        return response;
    }


    /// <summary>
    /// Validar si tiene acceso a una política.
    /// </summary>
    /// <param name="policy">Id de la política.</param>
    /// <param name="identity">Id de la identidad.</param>
    [HttpGet("identity")]
    public async Task<HttpResponseBase> IsAllow([FromQuery] string policy, [FromHeader] int identity)
    {
        var response = await policiesData.HasFor(identity, policy);
        return response;
    }


}