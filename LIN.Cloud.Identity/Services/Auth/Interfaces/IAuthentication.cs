namespace LIN.Cloud.Identity.Services.Auth.Interfaces;

public interface IAuthentication
{


    /// <summary>
    /// Iniciar el proceso.
    /// </summary>
    public Task<Responses> Start();



    /// <summary>
    /// Establecer credenciales.
    /// </summary>
    /// <param name="username">Usuario único.</param>
    /// <param name="password">Contraseña.</param>
    /// <param name="appCode">Código de app.</param>
    public void SetCredentials(string username, string password, string appCode);



    /// <summary>
    /// Obtener el token.
    /// </summary>
    public string GenerateToken();



    /// <summary>
    /// Obtener la data.
    /// </summary>
    public AccountModel GetData();

}