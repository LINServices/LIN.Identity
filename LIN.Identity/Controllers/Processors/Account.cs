namespace LIN.Identity.Controllers.Processors;


public class AccountProcessor
{


    /// <summary>
    /// Procesa la información de un Account
    /// </summary>
    /// <param name="modelo">Modelo</param>
    public static AccountModel Process(AccountModel modelo)
    {

        var model = new AccountModel
        {
            ID = 0,
            Nombre = modelo.Nombre,
            Genero = modelo.Genero,
            OrganizationAccess = modelo.OrganizationAccess,
            Usuario = modelo.Usuario,
            Visibilidad = modelo.Visibilidad,
            Contraseña = modelo.Contraseña = EncryptClass.Encrypt(Conexión.SecreteWord + modelo.Contraseña),
            Creación = modelo.Creación = DateTime.Now,
            Estado = modelo.Estado = AccountStatus.Normal,
            Insignia = modelo.Insignia = AccountBadges.None,
            Rol = modelo.Rol = AccountRoles.User,
            Perfil = modelo.Perfil = modelo.Perfil.Length == 0
                               ? File.ReadAllBytes("wwwroot/profile.png")
                               : modelo.Perfil,
        };

        model.Perfil = Image.Zip(model.Perfil);
        return model;

    }


}
