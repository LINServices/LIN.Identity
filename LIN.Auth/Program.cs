global using System.Text;
global using Microsoft.Data.SqlClient;
global using Http.ResponsesList;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.SqlServer;
global using Microsoft.IdentityModel.Tokens;
global using LIN.Types.Enumerations;
global using LIN.Auth.Services;
global using LIN.Types.Responses;
global using LIN.Types.Auth.Enumerations;
global using LIN.Types.Auth.Models;
global using LIN.Modules;
global using Microsoft.AspNetCore.Mvc;
using LIN.Auth.Data;
using LIN.Auth;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

string sqlConnection = builder.Configuration["ConnectionStrings:somee"] ?? string.Empty;

// Servicio de BD
builder.Services.AddDbContext<Context>(options =>
{
    options.UseSqlServer(sqlConnection);
});



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


try
{
    // Si la base de datos no existe
    using var scope = app.Services.CreateScope();
    var dataContext = scope.ServiceProvider.GetRequiredService<Context>();
    var res = dataContext.Database.EnsureCreated();
}
catch
{ }

app.UseSwagger();
app.UseSwaggerUI();

Conexión.SetStringConnection(sqlConnection);


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
