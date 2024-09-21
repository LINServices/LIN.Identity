namespace LIN.Cloud.Identity.Services.Models;

public class JwtModel
{

    /// <summary>
    /// El token esta autenticado.
    /// </summary>
    public bool IsAuthenticated { get; set; }


    /// <summary>
    /// Usuario.
    /// </summary>
    public string Unique { get; set; } = string.Empty;


    /// <summary>
    /// Id de la cuenta.
    /// </summary>
    public int AccountId { get; set; }


    /// <summary>
    /// Id de la identidad.
    /// </summary>
    public int IdentityId { get; set; }


    /// <summary>
    /// Id de la aplicación.
    /// </summary>
    public int ApplicationId { get; set; }

}