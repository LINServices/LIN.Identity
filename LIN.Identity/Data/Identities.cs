namespace LIN.Identity.Data;


public class Identities
{


    #region Abstracciones




    public static async Task<ReadOneResponse<IdentityModel>> Read(int identity)
    {
        var (context, contextKey) = Conexión.GetOneConnection();

        var res = await Read(identity, context);
        context.CloseActions(contextKey);
        return res;
    }




    #endregion




    public static async Task<ReadOneResponse<IdentityModel>> Read(int id, Conexión context)
    {

        // Ejecución
        try
        {

            var ids = await (from identity in context.DataBase.Identities
                                  where identity.Id == id   
                                  select identity).FirstOrDefaultAsync();


            if (ids == null)
                return new(Responses.NotRows);

            return new(Responses.Success, ids);

        }
        catch
        {
        }

        return new();
    }


}