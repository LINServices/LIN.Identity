using Newtonsoft.Json;

namespace LIN.Cloud.Identity.Persistence.Contexts;

internal class SeedContext
{

    /// <summary>
    /// Data primaria.
    /// </summary>
    public static void Seed(DataContext context)
    {
        if (!context.Accounts.Any())
        {
            var jsonData = File.ReadAllText("wwwroot/seeds/users.json");
            var users = JsonConvert.DeserializeObject<List<AccountModel>>(jsonData) ?? [];

            foreach (var user in users)
                user.Password = Global.Utilities.Cryptography.Encrypt(user.Password);

            if (users != null && users.Count > 0)
            {
                context.Accounts.AddRange(users);
                context.SaveChanges();
            }
        }

        if (!context.Applications.Any())
        {
            var jsonData = File.ReadAllText("wwwroot/seeds/applications.json");
            var apps = JsonConvert.DeserializeObject<List<ApplicationModel>>(jsonData) ?? [];

            foreach (var app in apps)
            {
                app.Identity.Type = Types.Cloud.Identity.Enumerations.IdentityType.Service;
                app.Owner = new() { Id = app.OwnerId };
                app.Owner = context.AttachOrUpdate(app.Owner)!;
            }

            if (apps != null && apps.Count > 0)
            {
                context.Applications.AddRange(apps);
                context.SaveChanges();
            }
        }
    }

}