﻿namespace LIN.Cloud.Identity.Areas.Policies;

[IdentityToken]
[Route("[controller]")]
public class PoliciesController(Data.Policies policiesData, Data.Groups groups, RolesIam iam) : AuthenticationBaseController
{

    /// <summary>
    /// Crear nueva política.
    /// </summary>
    /// <param name="modelo">Modelo de la identidad.</param>
    [HttpPost]
    public async Task<HttpCreateResponse> Create([FromBody] PolicyModel modelo)
    {

        // Si ya tiene una identidad.
        if (modelo.OwnerIdentityId > 0)
        {
            // Obtener detalles.
            var owner = await groups.GetOwnerByIdentity(modelo.OwnerIdentityId);

            if (owner.Response != Responses.Success)
                return new(Responses.NotRows) { Message = $"No se encontró la organización del grupo con identidad {modelo.OwnerIdentityId}" };

            // Validar roles.
            var roles = await iam.RolesOn(AuthenticationInformation.IdentityId, owner.Model);

            bool hasPermission = ValidateRoles.ValidateAlterMembers(roles);

            if (!hasPermission)
                return new(Responses.Unauthorized) { Message = $"No tienes permisos para crear políticas a titulo de la organización #{owner.Model}." };

        }
        else
        {
            // Establecer propietario al usuario que realiza la solicitud.
            modelo.OwnerIdentityId = AuthenticationInformation.IdentityId;
        }

        // Formatear.
        modelo.OwnerIdentity = new()
        {
            Id = modelo.OwnerIdentityId
        };

        modelo.ApplyFor = [];

        var response = await policiesData.Create(modelo);
        return response;
    }


    /// <summary>
    /// Obtener políticas asociadas a una cuenta.
    /// </summary>
    [HttpGet("all")]
    public async Task<HttpReadAllResponse<PolicyModel>> All()
    {
        var response = await policiesData.ReadAllOwn(AuthenticationInformation.IdentityId);
        return response;
    }


    /// <summary>
    /// Obtener políticas asociadas a una organización.
    /// </summary>
    [HttpGet("organization/all")]
    public async Task<HttpReadAllResponse<PolicyModel>> OrganizationAll([FromHeader] int organization)
    {
        var response = await policiesData.ReadAll(organization);
        return response;
    }


    /// <summary>
    /// Eliminar política.
    /// </summary>
    /// <param name="policy">Id de la política.</param>
    [HttpDelete]
    public async Task<HttpResponseBase> Delete([FromQuery] string policy)
    {

        // Validar Iam.
       var iamResult = await iam.IamPolicy(AuthenticationInformation.IdentityId, policy);

        if (iamResult != Types.Enumerations.IamLevels.Privileged)
            return new ResponseBase(Responses.Unauthorized) { Message = "No tienes permisos para eliminar la política." };

        var response = await policiesData.Remove(policy);
        return response;
    }

}