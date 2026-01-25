using Http.Extensions;
using Http.Extensions.OpenApi;
using LIN.Cloud.Identity.Services.Realtime;

var builder = WebApplication.CreateBuilder(args);

// Servicios de contenedor.
builder.Services.AddLINHttp(true, (options) =>
{
    options.CustomSchemaIds(type => type.FullName);
    options.OperationFilter<HeaderMapAttribute<IdentityTokenAttribute>>("token", "Token de acceso a LIN Cloud Identity");
});

builder.Services.AddSignalR();
builder.Services.AddLocalServices();
builder.Services.AddAuthenticationServices(builder.Configuration);

// Servicio de autenticación.
builder.Services.AddPersistence(builder.Configuration);

var app = builder.Build();
app.UseLINHttp(useGateway: true);

// Base de datos.
app.UseDataBase();

JwtService.Open(builder.Configuration);

// Hub.
app.MapHub<PassKeyHub>("/realTime/auth/passkey");

app.UseAuthorization();
app.MapControllers();

app.Run();