namespace LIN.Cloud.Identity.Persistence.Formatters;

public class Identities
{

    /// <summary>
    /// Procesar el modelo.
    /// </summary>
    /// <param name="id">Modelo</param>
    public static void Process(IdentityModel id)
    {
        id.Id = 0;
        id.ExpirationTime = DateTime.UtcNow.AddYears(10);
        id.EffectiveTime = DateTime.UtcNow;
        id.CreationTime = DateTime.UtcNow;
        id.Status = IdentityStatus.Enable;
        id.Unique = id.Unique.Trim();
    }

}