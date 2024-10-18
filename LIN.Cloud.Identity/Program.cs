using Http.Extensions;
using LIN.Cloud.Identity.Services.Auth.Interfaces;
using LIN.Cloud.Identity.Services.Extensions;
using LIN.Cloud.Identity.Services.Realtime;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

// Servicios de contenedor.
builder.Services.AddSignalR();
builder.Services.AddLINHttp(true, (options) =>
{
    options.OperationFilter<CustomOperationFilter<IdentityTokenAttribute>>("token");
});

builder.Services.AddLocalServices();


// Servicio de autenticación.
builder.Services.AddScoped<IAuthentication, Authentication>();
builder.Services.AddPersistence(builder.Configuration);

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