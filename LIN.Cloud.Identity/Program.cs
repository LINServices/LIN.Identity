using Http.Extensions;
using Http.Extensions.OpenApi;
using LIN.Cloud.Identity.Services.Realtime;

var builder = WebApplication.CreateBuilder(args);

// Servicios de contenedor.
builder.Services.AddLINHttp(true, (options) =>
{
    options.OperationFilter<HeaderMapAttribute<IdentityTokenAttribute>>("token", "Token de acceso a LIN Cloud Identity");
});

builder.Services.AddSignalR();
builder.Services.AddLocalServices();
builder.Services.AddAuthenticationServices(builder.Configuration);

// Servicio de autenticación.
builder.Services.AddPersistence(builder.Configuration);
//builder.Host.UseLoggingService(builder.Configuration);

var app = builder.Build();
app.UseLINHttp();

// Base de datos.
app.UseDataBase();

// Hub.
app.MapHub<PassKeyHub>("/realTime/auth/passkey");

app.UseAuthorization();
app.MapControllers();

builder.Services.AddDatabaseAction(() =>
{
    var context = app.Services.GetRequiredService<DataContext>();
    context.Accounts.Where(x => x.Id == 0).FirstOrDefaultAsync();
    return "Success";
});

app.Run();