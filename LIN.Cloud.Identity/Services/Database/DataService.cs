namespace LIN.Cloud.Identity.Services.Database;


public static class DataService
{


    /// <summary>
    /// Cache de conexiones.
    /// </summary>
    private static List<DataBase> Cache { get; set; } = [];


    /// <summary>
    /// Cantidad de instancias de base de datos.
    /// </summary>
    public static int MaxInstances { get; set; } = 10;


    /// <summary>
    /// Cadena de conexión SQL Server.
    /// </summary>
    public static string ConnectionString { get; private set; } = string.Empty;



    /// <summary>
    /// Agregar base de datos al cache.
    /// </summary>
    /// <param name="db">Base de datos.</param>
    public static void AddToCache(DataBase db)
    {
        if (Cache.Count >= MaxInstances)
            return;
        Cache.Add(db);
    }



    /// <summary>
    /// Obtener una conexión a base de datos.
    /// </summary>
    public static (DataBase context, string contextKey) GetConnection()
    {

        // Obtener base de datos del cache.
        var cache = Cache.FirstOrDefault(db => !db.OnUseAction);

        // Validaciones.
        if (cache != null && cache.Key == string.Empty)
        {
            // Bloquear el objecto.
            lock (cache)
            {
                cache.SetOnUse();
                string key = Global.Utilities.KeyGenerator. Generate(10, "db.");
                cache.Key = key;
                return (cache, key);
            }
        }

        // Generar estancia forzosa.
        var newDB = new DataBase()
        {
            Key = Global.Utilities.KeyGenerator.Generate(10, "db.")
        };

        newDB.SetOnUse();
        AddToCache(newDB);
        return (newDB, newDB.Key);

    }



    /// <summary>
    /// Obtener una conexión a base de datos.
    /// </summary>
    public static (DataBase context, string contextKey) GetConnectionForce()
    {
        return (new(), "");

    }



    /// <summary>
    /// Habilitar el servicio de base de datos.
    /// </summary>
    public static IServiceCollection AddDataBase(this IServiceCollection context, string sql)
    {
        ConnectionString = sql;
        context.AddDbContext<DataContext>(options =>
        {
            options.UseSqlServer(sql);
#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        });

        return context;
    }



    /// <summary>
    /// Habilitar el servicio de base de datos.
    /// </summary>
    public static IApplicationBuilder UseDataBase(this IApplicationBuilder context)
    {
        try
        {
            var (connection, key) = GetConnection();
            bool created = connection.Database.EnsureCreated();
            connection.Close(key);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        return context;
    }


}