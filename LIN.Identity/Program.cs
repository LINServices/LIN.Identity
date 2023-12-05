using LIN.Identity.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.OpenApi.Models;
using System.ComponentModel;


{

    LIN.Access.Logger.Logger.AppName = "LIN.IDENTITY";

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



    builder.Services.AddControllers(options =>
    {
        options.Conventions.Add(new GroupingByNamespaceConvention());
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
    builder.Services.AddSwaggerGen((config) =>
    {
        var titleBase = "LIN Identity";
        var description = "IDENTITY";
        var TermsOfService = new Uri("http://linapps.co/");
        var License = new OpenApiLicense()
        {
            Name = "MIT"
        };

        var Contact = new OpenApiContact()
        {
            Name = "Alexander Giraldo",
            Email = "",
            Url = new Uri("http://linapps.co/")
        };

        config.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = titleBase + " v1",
            Description = description,
            TermsOfService = TermsOfService,
            License = License,
            Contact = Contact
        });
        config.SwaggerDoc("v3", new OpenApiInfo
        {
            Version = "v3",
            Title = titleBase + " v3",
            Description = description,
            TermsOfService = TermsOfService,
            License = License,
            Contact = Contact
        });
    });

    var app = builder.Build();


    try
    {
        // Si la base de datos no existe
        using var scope = app.Services.CreateScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<Context>();
        var res = dataContext.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        _ = LIN.Access.Logger.Logger.Log(ex, 3);
    }


    app.UseCors("AllowAnyOrigin");

    app.MapHub<AccountHub>("/realTime/service");
    app.MapHub<PassKeyHub>("/realTime/auth/passkey");

    app.UseSwagger();
    app.UseSwaggerUI(config =>
    {
        config.SwaggerEndpoint("/swagger/v1/swagger.json", "MoviesAPI v1");
        config.SwaggerEndpoint("/swagger/v3/swagger.json", "MoviesAPI v3");
    });

    Conexión.SetStringConnection(sqlConnection);

    app.UseStaticFiles();
    app.UseHttpsRedirection();
    Jwt.Open();
    EmailWorker.StarService();


    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}