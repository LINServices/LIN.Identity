global using System.Text;
global using Microsoft.Data.SqlClient;
global using Http.ResponsesList;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.SqlServer;
global using Microsoft.IdentityModel.Tokens;
global using LIN.Types.Enumerations;
global using LIN.Identity.Services;
global using LIN.Types.Responses;
global using LIN.Types.Auth.Enumerations;
global using LIN.Types.Auth.Models;
global using LIN.Modules;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.SignalR;
global using LIN.Identity.Hubs;
global using LIN.Identity;

using LIN.Identity.Data;
{

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

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




    var sqlConnection = builder.Configuration["ConnectionStrings:somee"] ?? string.Empty;

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
    {
    }


    app.UseCors("AllowAnyOrigin");

    app.MapHub<AccountHub>("/realTime/service");
    app.MapHub<PassKeyHub>("/realTime/auth/passkey");

    app.UseSwagger();
    app.UseSwaggerUI();

    Conexión.SetStringConnection(sqlConnection);

    app.UseStaticFiles();
    app.UseHttpsRedirection();
    Jwt.Open();
    EmailWorker.StarService();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}