using LIN.Cloud.Identity.Services.Realtime;

var builder = WebApplication.CreateBuilder(args);

// Servicios de contenedor.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

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

var app = builder.Build();

// Middlewares personalizados.
app.UseIP();



app.UseCors("AllowAnyOrigin");


// Swagger.
app.UseSwagger();
app.UseSwaggerUI();

// Base de datos.
app.UseDataBase();

// Hub.
app.MapHub<PassKeyHub>("/realTime/auth/passkey");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
