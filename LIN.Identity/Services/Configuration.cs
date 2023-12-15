namespace LIN.Identity.Services;


public class Configuration
{



    private static IConfiguration? Config;

    private static bool _isStart = false;


    public static string GetConfiguration(string route)
    {

        if (_isStart && Config != null)
            return Config[route] ?? string.Empty;

        var configBuilder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", false, true);

        Config = configBuilder.Build();
        _isStart = true;

        return Config[route] ?? string.Empty;

    }

}