namespace LIN.Identity.Services.Iam;


public static class Applications
{


    /// <summary>
    /// Validar acceso a un recurso de aplicación.
    /// </summary>
    /// <param name="account">ID de la cuenta</param>
    /// <param name="app">ID de la aplicación</param>
    public static async Task<ReadOneResponse<IamLevels>> ValidateAccess(int account, int app)
    {


        // Conexión.
        var (context, contextKey) = Conexión.GetOneConnection();


        var identity = (from a in context.DataBase.Accounts
                        where a.ID == account
                        select a.IdentityId).FirstOrDefault();





        List<DirectoryMember> final = new List<DirectoryMember>();



        await A(identity, final);



        var mapp = final.Select(t => t.DirectoryId).ToList();


        var policies = (from p in context.DataBase.Policies
                        where mapp.Contains(p.DirectoryId)
                        select p).Distinct();

        var q = policies.ToQueryString();
        
        
        var ssss = policies.ToList();


        // Obtiene el recurso.
        var resource = await Data.Applications.Read(app);

        // Si no existe el recurso.
        if (resource == null)
            return new()
            {
                Message = "No se encontró el recurso.",
                Response = Responses.NotRows,
                Model = IamLevels.NotAccess
            };


        // App publica.
        if (resource.Model.DirectoryId <= 0)
        {
            return new()
            {
                Response = Responses.Success,
                Model = IamLevels.Visualizer
            };
        }

















        return new()
        {
            Response = Responses.NotRows,
            Model = IamLevels.NotAccess,
            Message = "TESTING."
        };




        context.CloseActions(contextKey);


        //if (directory == null)
        //{
        //    return new()
        //    {
        //        Response = Responses.NotRows,
        //        Model = IamLevels.NotAccess,
        //        Message = "No tienes acceso a este recurso/app."
        //    };
        //}


        //return new()
        //{
        //    Response = Responses.Success,
        //    Model = (directory.Rol == DirectoryRoles.Administrator) ? IamLevels.Privileged : IamLevels.Visualizer
        //};

    }



    static async Task A(int identidad, List<DirectoryMember> final, Conexión context)
    {


        var x = from DM in context.DataBase.DirectoryMembers
                where DM.IdentityId == identidad
                select new DirectoryMember
                {
                    DirectoryId = DM.DirectoryId,
                    Directory = new()
                    {
                        ID = DM.Directory.ID,
                        Identity = new()
                        {
                            Id = DM.Directory.Identity.Id
                        }
                    }
                };


        if (x.Any())
        {
            var ss = x.ToList();
            final.AddRange(ss);

            foreach (var member in ss)
            {
                await A(member.Directory.Identity.Id, final, context);
            }

        }

    }


}
