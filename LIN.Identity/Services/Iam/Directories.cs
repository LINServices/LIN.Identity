namespace LIN.Identity.Services.Iam;


public static class Directories
{





    public static async Task<ReadOneResponse<IamLevels>> ValidateAccess(int identity, int directory)
    {


        var (_, identities) = await Queries.Directories.Get(identity);



        var (context, contextKey) = Conexión.GetOneConnection();


        // Encuentra el acceso en order Admin -> User.
        var dirM = (from DM in context.DataBase.DirectoryMembers
                    where DM.DirectoryId == directory
                    && identities.Contains(DM.IdentityId)
                    select DM).OrderByDescending(t => t.Rol).FirstOrDefault();

        if (dirM == null)
            return new()
            {
                Model = IamLevels.NotAccess
            };

        if (dirM.Rol == DirectoryRoles.Administrator)
            return new()
            {
                Model = IamLevels.Privileged
            };

        return new()
        {
            Model = IamLevels.Visualizer
        };


    }



}