namespace LIN.Cloud.Identity.Services.Formats;

public class Identities
{

    /// <summary>
    /// Procesar el modelo.
    /// </summary>
    /// <param name="id">Modelo</param>
    public static void Process(IdentityModel id)
    {
        id.Id = 0;
        id.ExpirationTime = DateTime.Now.AddYears(10);
        id.EffectiveTime = DateTime.Now;
        id.CreationTime = DateTime.Now;
        id.Status = IdentityStatus.Enable;
        id.Unique = id.Unique.Trim();
    }

}