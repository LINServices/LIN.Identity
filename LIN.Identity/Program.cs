using LIN.Identity.Data;


try
{

    // Nombre de la app en Error Logger.
    Logger.AppName = "LIN.IDENTITY.V3";

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
    catch (Exception ex)
    {
        _ = LIN.Access.Logger.Logger.Log(ex, 3);
    }


    app.UseCors("AllowAnyOrigin");

    app.MapHub<PassKeyHub>("/realTime/auth/passkey");

    app.UseSwagger();
    app.UseSwaggerUI();

    Conexión.SetStringConnection(sqlConnection);

    app.UseStaticFiles();
    app.UseHttpsRedirection();
    Jwt.Open();
    EmailWorker.StarService();



    app.MapGet("/", () => "Hello World!");

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Logger.AppName = "LIN.Identity.V3";
    await Logger.Log(ex, 3);
}