using LIN.Cloud.Identity.Services.Realtime;
using Http.Services;
using Http.Extensions;
using LIN.Cloud.Identity.Services.Auth.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Servicios de contenedor.
builder.Services.AddSignalR();
builder.Services.AddLINHttp();

// Servicios personalizados
string sql = "";

#if DEBUG
sql = builder.Configuration["ConnectionStrings:cloud"] ?? string.Empty;
#elif RELEASE
sql = builder.Configuration["ConnectionStrings:cloud"] ?? string.Empty;
#endif

builder.Services.AddDataBase(sql);

// Servicios propios.
builder.Services.AddIP();

// Servicio de autenticación.
builder.Services.AddScoped<IAuthentication, Authentication>();

var app = builder.Build();

// Middlewares personalizados.
app.UseIP();

app.UseLINHttp();

// Base de datos.
app.UseDataBase();

// Hub.
app.MapHub<PassKeyHub>("/realTime/auth/passkey");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
