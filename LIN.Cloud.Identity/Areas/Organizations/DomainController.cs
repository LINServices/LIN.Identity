using DnsClient;
using System.Text.RegularExpressions;

namespace LIN.Cloud.Identity.Areas.Organizations;

[IdentityToken]
[Route("[controller]")]
public class DomainController(IDomainRepository domainRepository, IIamService iamService, IDomainService domainService) : AuthenticationBaseController
{

    /// <summary>
    /// Agrega un dominio a una organización.
    /// </summary>
    [HttpPost]
    public async Task<CreateResponse> AddDomain([FromHeader] int organization, [FromQuery] string domain)
    {

        // Validar dominio sea valido.
        if (!domainService.VerifyDomain(domain))
            return new(Responses.InvalidParam)
            {
                Message = "El dominio no es valido."
            };

        // Validar si el usuario tiene permisos sobre la organización.
        var roles = await iamService.Validate(base.UserInformation.IdentityId, organization);

        if (!roles.ValidateAlterDomain())
            return new(Responses.Unauthorized)
            {
                Message = "No tienes permisos para agregar un dominio a esta organización."
            };

        // Código de verificación para el dominio.
        string verificationCode = Guid.NewGuid().ToString("N").ToLowerInvariant();

        // Crear el dominio en la organización.
        var modelo = new DomainModel
        {
            Organization = new() { Id = organization },
            Domain = domain.ToLowerInvariant(),
            IsVerified = false,
            VerificationCode = verificationCode
        };

        var response = await domainRepository.Create(modelo);

        if (response.Response != Responses.Success)
            return new(Responses.Unauthorized)
            {
                Message = "No se pudo crear el dominio. Intenta nuevamente."
            };

        // Retornar el código para el TXT del dominio.
        return new(Responses.Success)
        {
            LastUnique = verificationCode,
            Message = "Dominio creado correctamente. Agrega el TXT con el código de verificación en tu dominio."
        };
    }


    /// <summary>
    /// Verifica un dominio de una organización.
    /// </summary>
    [HttpPatch]
    public async Task<ResponseBase> Verify([FromHeader] int organization, [FromQuery] string domain)
    {
        // Validar dominio sea valido.
        if (!domainService.VerifyDomain(domain))
            return new(Responses.InvalidParam)
            {
                Message = "El dominio no es valido."
            };

        // Validar si el usuario tiene permisos sobre la organización.
        var roles = await iamService.Validate(base.UserInformation.IdentityId, organization);

        if (!roles.ValidateAlterDomain())
            return new(Responses.Unauthorized)
            {
                Message = "No tienes permisos para agregar un dominio a esta organización."
            };

        // Obtener el dominio desde la base de datos.
        var response = await domainRepository.Read(domain.ToLowerInvariant());

        if (response.Response != Responses.Success)
            return new(Responses.NotRows)
            {
                Message = "El dominio no existe o no pertenece a la organización."
            };

        // Verificar el TXT.
        var isSuccess = await domainService.VerifyDns(domain, response.Model.VerificationCode);

        if (!isSuccess)
            return new(Responses.Unauthorized)
            {
                Message = "El dominio no ha sido verificado. Asegúrate de agregar el registro TXT con el código de verificación."
            };

        // Es valido, se marca el dominio como verificado.
        var verifyResponse = await domainRepository.Verify(domain.ToLowerInvariant());

        // Si no es valido, se retorna un error.
        return verifyResponse.Response == Responses.Success
            ? new(Responses.Success)
            {
                Message = "Dominio verificado correctamente."
            }
            : new(Responses.Unauthorized)
            {
                Message = "No se pudo verificar el dominio. Intenta nuevamente."
            };
    }

}