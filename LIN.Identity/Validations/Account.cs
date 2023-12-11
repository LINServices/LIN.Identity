namespace LIN.Identity.Validations;


public class Account
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
            OrganizationAccess = modelo.OrganizationAccess,
            Identity = new()
            {
                Id = 0,
                Type = IdentityTypes.Account,
                Unique = modelo.Identity.Unique
            },
            
            IdentityId = 0,
            Visibilidad = modelo.Visibilidad,
            Contraseña = modelo.Contraseña = EncryptClass.Encrypt(modelo.Contraseña),
            Creación = modelo.Creación = DateTime.Now,
            Estado = modelo.Estado = AccountStatus.Normal,
            Birthday = modelo.Birthday,
            Insignia = modelo.Insignia = AccountBadges.None,
            Rol = modelo.Rol = AccountRoles.User,
            Perfil = modelo.Perfil = modelo.Perfil.Length == 0
                ? File.ReadAllBytes("wwwroot/profile.png")
                : modelo.Perfil
        };

        model.Perfil = Image.Zip(model.Perfil);
        return model;

    }


}