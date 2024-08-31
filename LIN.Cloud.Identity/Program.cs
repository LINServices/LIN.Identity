using Http.Extensions;
using LIN.Cloud.Identity.Persistence.Extensions;
using LIN.Cloud.Identity.Services.Auth.Interfaces;
using LIN.Cloud.Identity.Services.Extensions;
using LIN.Cloud.Identity.Services.Realtime;

var builder = WebApplication.CreateBuilder(args);

// Servicios de contenedor.
builder.Services.AddSignalR();
builder.Services.AddLINHttp();

// Servicios propios.
builder.Services.AddIP();
builder.Services.AddLocalServices();

// Servicio de autenticación.
builder.Services.AddScoped<IAuthentication, Authentication>();
builder.Services.AddPersistence(builder.Configuration);

var app = builder.Build();

// Middlewares personalizados.
app.UseIP();

app.UseLINHttp();

// Base de datos.
app.UseDataBase();

// Hub.
app.MapHub<PassKeyHub>("/realTime/auth/passkey");

app.UseAuthorization();
app.MapControllers();

app.Run();
