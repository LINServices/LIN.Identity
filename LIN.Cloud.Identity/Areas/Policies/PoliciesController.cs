using LIN.Cloud.Identity.Persistence.Repositories;

namespace LIN.Cloud.Identity.Areas.Policies;

[IdentityToken]
[Route("[controller]")]
public class PoliciesController(IPolicyRepository policiesData, IGroupRepository groups, IamRoles iam, IOrganizationRepository organizations, IamPolicy iamPolicy) : AuthenticationBaseController
{

    /// <summary>
    /// Crear nueva política.
    /// </summary>
    /// <param name="modelo">Modelo de la identidad.</param>
    [HttpPost]
    public async Task<HttpCreateResponse> Create([FromBody] PolicyModel modelo, [FromHeader] int? organization, [FromHeader] bool assign)
    {

        //// Si ya tiene una identidad.
        //if (modelo.OwnerIdentityId > 0 && (organization is null || organization <= 0))
        //{
        //    // Obtener detalles.
        //    var owner = await groups.GetOwnerByIdentity(modelo.OwnerIdentityId);

        //    if (owner.Response != Responses.Success)
        //        return new(Responses.NotRows) { Message = $"No se encontró la organización del grupo con identidad {modelo.OwnerIdentityId}" };

        //    // Validar roles.
        //    var roles = await iam.Validate(UserInformation.IdentityId, owner.Model);

        //    bool hasPermission = ValidateRoles.ValidateAlterMembers(roles);

        //    if (!hasPermission)
        //        return new(Responses.Unauthorized) { Message = $"No tienes permisos para crear políticas a titulo de la organización #{owner.Model}." };

        //}
        //else if (organization is not null && organization > 0)
        //{
        //    // Validar roles.
        //    var roles = await iam.Validate(UserInformation.IdentityId, organization.Value);

        //    bool hasPermission = ValidateRoles.ValidateAlterMembers(roles);

        //    if (!hasPermission)
        //        return new(Responses.Unauthorized) { Message = $"No tienes permisos para crear políticas a titulo de la organización #{organization}." };

        //    // 
        //    var directoryIdentity = await organizations.ReadDirectoryIdentity(organization.Value);
        //    modelo.OwnerIdentityId = directoryIdentity.Model;
        //}
        //else
        //{
        //    // Establecer propietario al usuario que realiza la solicitud.
        //    modelo.OwnerIdentityId = UserInformation.IdentityId;
        //}

        //// Formatear.
        //modelo.OwnerIdentity = new()
        //{
        //    Id = modelo.OwnerIdentityId
        //};

        //modelo.ApplyFor = [];
        //if (assign)
        //    modelo.ApplyFor = [new() {
        //        Identity = new(){
        //            Id = modelo.OwnerIdentityId
        //        }
        //    }];

        var response = await policiesData.Create(modelo);
        return response;
    }





}