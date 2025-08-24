using Newtonsoft.Json;

namespace LIN.Cloud.Identity.Persistence.Contexts;

internal class SeedContext
{

    /// <summary>
    /// Data primaria.
    /// </summary>
    public static void Seed(DataContext context)
    {
        // Si no hay cuentas.
        if (!context.Accounts.Any())
        {
            // Obtener la data.
            var jsonData = File.ReadAllText("wwwroot/seeds/users.json");
            var users = JsonConvert.DeserializeObject<List<AccountModel>>(jsonData) ?? [];

            foreach (var user in users)
                user.Password = Global.Utilities.Cryptography.Encrypt(user.Password);

            // Agregar los modelos.
            if (users != null && users.Count > 0)
            {
                context.Accounts.AddRange(users);
                context.SaveChanges();
            }
        }

        // Si no hay aplicaciones.
        if (!context.Applications.Any())
        {
            // Obtener la data.
            var jsonData = File.ReadAllText("wwwroot/seeds/applications.json");
            var apps = JsonConvert.DeserializeObject<List<ApplicationModel>>(jsonData) ?? [];

            // Formatear modelos.
            foreach (var app in apps)
            {
                app.Identity.Type = Types.Cloud.Identity.Enumerations.IdentityType.Service;
                app.Owner = new() { Id = app.OwnerId };
                app.Owner = context.AttachOrUpdate(app.Owner)!;
            }

            // Agregar aplicaciones.
            if (apps != null && apps.Count > 0)
            {
                context.Applications.AddRange(apps);
                context.SaveChanges();
            }
        }
    }

}