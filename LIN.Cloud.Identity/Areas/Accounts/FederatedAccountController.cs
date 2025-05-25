using System.Text.RegularExpressions;

namespace LIN.Cloud.Identity.Areas.Accounts;

[Route("[controller]")]
public class FederatedAccountController(ITemporalAccountRepository temporalAccountRepository, IIamService iamService, IDomainRepository domainRepository, IAccountRepository accountRepository, IIdentityRepository identityRepository, EmailSender emailSender, IGroupMemberRepository groupMemberRepository, IOrganizationRepository organizationRepository) : AuthenticationBaseController
{

    /// <summary>
    /// Agrega una cuenta federada a la organización.
    /// </summary>
    /// <param name="organization">Id de la organización.</param>
    /// <param name="email">Correo electrónico.</param>
    /// <param name="provider">Proveedor.</param>
    [HttpPost]
    [IdentityToken]
    public async Task<ResponseBase> AddFederatedAccount([FromHeader] int organization, [FromQuery] string email, [FromQuery] IdentityService provider)
    {
        // 1. Obtener el dominio del correo
        var match = Regex.Match(email, @"@(?<domain>[^@]+)$");
        if (!match.Success)
            return new(Responses.InvalidParam)
            {
                Message = "El correo electrónico no es válido."
            };

        var domain = match.Groups["domain"].Value.ToLowerInvariant();

        // 2. Validar si la organización es dueña del dominio
        var domainResponse = await domainRepository.Read(domain);
        bool isOrgOwner = domainResponse.Response == Responses.Success &&
                          domainResponse.Model.OrganizationId == organization &&
                          domainResponse.Model.IsVerified;

        // Validar que no exista una cuenta federada con el mismo correo.
        var exist = await identityRepository.Exist(email);

        if (exist.Response == Responses.Success && exist.Model)
            return new(Responses.ExistAccount)
            {
                Message = "Ya existe una cuenta federada con este correo."
            };

        if (!isOrgOwner)
        {

            // Crear la cuenta temporal.
            var tempAccount = new TemporalAccountModel
            {
                Email = email,
                Name = "Cuenta Federada " + email,
                Provider = provider,
                VerificationCode = Global.Utilities.KeyGenerator.Generate(9, "code."),
                OrganizationId = organization
            };

            // Guardar la cuenta temporal.
            await temporalAccountRepository.Create(tempAccount);

            // Enviar correo de invitación a la organización.
            await emailSender.Send(email, "Verifica tu cuenta federada", $"Por favor verifica tu correo para continuar. {tempAccount.VerificationCode}");
            return new(Responses.Success)
            {
                Message = "Se ha enviado un correo de verificación a " + email + ". Por favor verifica tu cuenta."
            };
        }

        var roles = await iamService.Validate(UserInformation.IdentityId, organization);
        if (!ValidateRoles.ValidateAlterMembers(roles))
            return new(Responses.Unauthorized)
            {
                Message = "No tienes permisos para agregar cuentas federadas internas."
            };

        // Crear la cuenta federada.
        var accountModel = new AccountModel
        {
            AccountType = AccountTypes.Work,
            Identity = new IdentityModel
            {
                Unique = email,
                Type = IdentityType.Account
            },
            Name = "Cuenta Federada " + email,
            Password = Global.Utilities.KeyGenerator.Generate(20, "pwd"),
            Visibility = Visibility.Visible
        };

        accountModel = Services.Formats.Account.Process(accountModel);
        accountModel.Identity.Provider = provider;

        // Crear la cuenta.
        var accountResponse = await accountRepository.Create(accountModel, organization);

        if (accountResponse.Response != Responses.Success)
            return new(Responses.Undefined)
            {
                Message = "Error al crear la cuenta federada."
            };

        return new(Responses.Success)
        {
            Message = "Cuenta federada creada exitosamente."
        };
    }


    /// <summary>
    /// Verificar una cuenta de terceros y agregarla al directorio de una organización.
    /// </summary>
    [HttpGet("verify")]
    public async Task<ResponseBase> Verify([FromQuery] string code)
    {
        // Validar si existe una cuenta temporal con el código.
        var temporalResponse = await temporalAccountRepository.ReadWithCode(code);

        if (temporalResponse.Response != Responses.Success)
            return new(Responses.NotRows)
            {
                Message = "No se encontró una cuenta con el código proporcionado."
            };

        // Validar que no exista una cuenta con la misma identidad.
        var accountExist = await accountRepository.Read(temporalResponse.Model.Email, new() { FindOn = FindOn.AllAccounts });

        if (accountExist.Response != Responses.Success)
        {
            // No existe una cuenta con la misma identidad, se procede a crear la cuenta federada.
            var accountModel = new AccountModel
            {
                AccountType = AccountTypes.Work,
                Identity = new IdentityModel
                {
                    Unique = temporalResponse.Model.Email,
                    Type = IdentityType.Account,
                    Provider = temporalResponse.Model.Provider
                },
                Name = temporalResponse.Model.Name,
                Password = Global.Utilities.KeyGenerator.Generate(20, "pwd"),
                Visibility = Visibility.Visible,
            };

            accountModel = Services.Formats.Account.Process(accountModel);
            accountModel.Identity.Provider = temporalResponse.Model.Provider;

            // Crear la cuenta.
            var accountResponse = await accountRepository.Create(accountModel, 0);
            accountExist = accountResponse;
        }

        // Validar que la cuenta se haya creado correctamente.
        if (accountExist.Response != Responses.Success)
            return new(Responses.Undefined)
            {
                Message = "Error al crear la cuenta federada."
            };

        // Eliminar la cuenta temporal.
        await temporalAccountRepository.Delete(temporalResponse.Model.Id);

        // Suscribir la cuenta al directorio de la organización.
        if (temporalResponse.Model.OrganizationId > 0)
        {
            // Obtener el directorio de la organización.
            var organization = await organizationRepository.ReadDirectory(temporalResponse.Model.OrganizationId);

            // Ingresarla a la organización.
            await groupMemberRepository.Create([new() {
                Group = new() { Id = organization.Model },
                Identity = new() { Id = accountExist.Model.IdentityId },
                Type = GroupMemberTypes.Guest
            }]);
        }

        return new(Responses.Success);
    }

}