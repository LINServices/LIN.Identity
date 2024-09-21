namespace LIN.Cloud.Identity.Services.Realtime;

public partial class PassKeyHub
{

    /// <summary>
    /// Construir el nombre de un grupo.
    /// </summary>
    /// <param name="user">Usuario.</param>
    public string BuildGroupName(string user) => $"gr.{user.ToLower().Trim()}";

}