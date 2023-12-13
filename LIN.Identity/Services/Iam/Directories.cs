namespace LIN.Identity.Services.Iam;


public static class Directories
{





    public static async Task<ReadOneResponse<IamLevels>> ValidateAccess(int identity, int directory)
    {


        var (_, identities) = await Queries.Directories.Get(identity);



        var (context, contextKey) = Conexión.GetOneConnection();



        var dirM = (from DM in context.DataBase.DirectoryMembers
                    where DM.DirectoryId == directory
                    && identities.Contains(DM.IdentityId)
                    select DM).GroupBy(b=>b.Rol).ToList();



        return new()
        {
            Model = IamLevels.NotAccess
        };


    }



}