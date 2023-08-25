namespace LIN.Auth.Controllers.Preparer;


public class Account
{
	public static AccountModel Preparar(AccountModel modelo)
	{

		var model = new AccountModel
		{
			ID = 0,
			Contraseña = modelo.Contraseña = EncryptClass.Encrypt(Conexión.SecreteWord + modelo.Contraseña),
			Creación = modelo.Creación = DateTime.Now,
			Estado = modelo.Estado = AccountStatus.Normal,
			Insignia = modelo.Insignia = AccountBadges.None,
			Rol = modelo.Rol = AccountRoles.User,
			Perfil = modelo.Perfil = modelo.Perfil.Length == 0
							   ? System.IO.File.ReadAllBytes("wwwroot/profile.png")
							   : modelo.Perfil,
		};

		return model;
		
	}
}
