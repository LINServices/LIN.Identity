namespace LIN.Identity.Validations;


public class AccountPassword
{


    /// <summary>
    /// Validar la contraseña con las políticas de directorio.
    /// </summary>
    /// <param name="account">Id de la cuenta.</param>
    /// <param name="password">Contraseña a validar.</param>
    public static async Task<bool> ValidatePassword(int account, string password)
    {

        // Contexto.
        var (context, contextKey) = Conexión.GetOneConnection();

        // Política.
        var policy = await (from directory in context.DataBase.DirectoryMembers
                            where directory.AccountId == account
                            join policie in context.DataBase.Policies
                            on directory.DirectoryId equals policie.DirectoryId
                            where policie.Type == PolicyTypes.PasswordLength
                            orderby policie.Creation
                            select new PolicyModel
                            {
                                Id = policie.Id,
                                ValueJson = policie.ValueJson,
                                Type = policie.Type
                            }).LastOrDefaultAsync();

        // Política no existe.
        if (policy == null)
            return true;

        try
        {

            // Valor.
            dynamic? policyValue = Newtonsoft.Json.JsonConvert.DeserializeObject(policy.ValueJson);

            // Convertir el valor.
            var can = int.TryParse(((policyValue?.length) as object)?.ToString(), out int length);

            // No se pudo pasear.
            if (!can)
                _ = Logger.Log($"Hubo un error al obtener el valor de Policy con id {policy.Id}", policy.ValueJson ?? "NULL", 3);

            // Validar.
            if (password.Length < length)
                return false;

        }
        catch (Exception)
        {
        }

        return true;

    }

}