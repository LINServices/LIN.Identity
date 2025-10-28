namespace LIN.Cloud.Identity.Services.Interfaces;

public interface IApplicationValidationService : IAuthenticationService;
public interface IIdentityValidationService : IAuthenticationService;
public interface IAccountValidationService : IAuthenticationService;
public interface IIdentityGetService : IAuthenticationService;
public interface IOrganizationValidationService : IAuthenticationService;

// Proveedores de servicios de autenticación de terceros (Google, Microsoft, etc.)
public interface IGoogleValidationService : IAuthenticationService;
public interface IMicrosoftValidationService : IAuthenticationService;
public interface IGitHubValidationService : IAuthenticationService;