namespace LIN.Identity.Data.Organizations;


public class Applications
{



    #region Abstractions



    /// <summary>
    /// Obtiene la lista de apps que coincidan con un patron y que no estén agregadas a una organización
    /// </summary>
    /// <param name="param">Parámetro de búsqueda</param>
    /// <param name="org">ID de la organización</param>
    public async static Task<ReadAllResponse<ApplicationModel>> Search(string param, int org)
    {
        var (context, contextKey) = Conexión.GetOneConnection();

        var res = await Search(param, org, context);
        context.CloseActions(contextKey);
        return res;
    }



    /// <summary>
    /// Crea una pp en la lista blanca de una organización
    /// </summary>
    /// <param name="appUid">UID de la aplicación</param>
    /// <param name="org">ID de la organización</param>
    public async static Task<CreateResponse> Create(string appUid, int org)
    {
        var (context, contextKey) = Conexión.GetOneConnection();

        var res = await Create(appUid, org, context);
        context.CloseActions(contextKey);
        return res;
    }



    /// <summary>
    /// Encuentra una app en una organización
    /// </summary>
    /// <param name="key">Key de la app</param>
    /// <param name="org">ID de la organización</param>
    public async static Task<ReadOneResponse<AppOnOrgModel>> AppOnOrg(string key, int org)
    {
        var (context, contextKey) = Conexión.GetOneConnection();

        var res = await AppOnOrg(key, org, context);
        context.CloseActions(contextKey);
        return res;
    }




    #endregion



    /// <summary>
    /// Encuentra una app en una organización
    /// </summary>
    /// <param name="key">Key de la app</param>
    /// <param name="org">ID de la organización</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ReadOneResponse<AppOnOrgModel>> AppOnOrg(string key, int org, Conexión context)
    {

        // Ejecución
        try
        {

            // Query
            var app = await (from E in context.DataBase.AppOnOrg
                             where E.Organization.ID == org
                             where E.App.Key == key
                             select E).FirstOrDefaultAsync();

            if (app == null)
                return new(Responses.NotRows);

            return new(Responses.Success, app);
        }
        catch
        {
        }

        return new();
    }



    /// <summary>
    /// Crea una app en una organización
    /// </summary>
    /// <param name="key">Key de la app</param>
    /// <param name="org">ID de la organización</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<CreateResponse> Create(string appUid, int org, Conexión context)
    {

        // Ejecución
        try
        {

            // Query
            var app = await (from A in context.DataBase.Applications
                             where A.ApplicationUid == appUid
                             select A).FirstOrDefaultAsync();


            if (app == null)
                return new(Responses.NotRows);


            var onOrg = new AppOnOrgModel()
            {
                State = AppOnOrgStates.Activated,
                App = app,
                Organization = new()
                {
                    ID = org
                }
            };
            context.DataBase.Attach(onOrg.Organization);

            await context.DataBase.AddAsync(onOrg);

            context.DataBase.SaveChanges();

            return new(Responses.Success, onOrg.ID);
        }
        catch
        {
        }

        return new();
    }



    /// <summary>
    /// Obtiene la lista de apps que coincidan con un patron y que no estén agregadas a una organización
    /// </summary>
    /// <param name="param">Parámetro de búsqueda</param>
	/// <param name="org">ID de la organización</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ReadAllResponse<ApplicationModel>> Search(string param, int org, Conexión context)
    {

        // Ejecución
        try
        {

            // Query
            var apps = await (from A in context.DataBase.Applications
                              where !context.DataBase.AppOnOrg.Any(aog => aog.AppID == A.ID && aog.OrgID == org)
                              where A.Name.ToLower().Contains(param.ToLower())
                              || A.ApplicationUid.ToLower().Contains(param.ToLower())
                              select new ApplicationModel
                              {
                                  ID = A.ID,
                                  ApplicationUid = A.ApplicationUid,
                                  Badge = A.Badge,
                                  Name = A.Name
                              }).Take(10).ToListAsync();


            return new(Responses.Success, apps);
        }
        catch
        {
        }

        return new();
    }



}
