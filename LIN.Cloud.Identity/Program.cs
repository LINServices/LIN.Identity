using LIN.Cloud.Identity.Services.Realtime;
using Http.Extensions;
using LIN.Cloud.Identity.Services.Auth.Interfaces;
using LIN.Cloud.Identity.Persistence.Extensions;

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

// Servicios propios.
builder.Services.AddIP();

// Servicio de autenticación.
builder.Services.AddScoped<IAuthentication, Authentication>();
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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
