namespace LIN.Auth.Data.Organizations;


public class Applications
{



	#region Abstractions


	public async static Task<CreateResponse> CreateOn(string key, int org)
	{
		var (context, contextKey) = Conexión.GetOneConnection();

		var res = await CreateOn(key, org, context);
		context.CloseActions(contextKey);
		return res;
	}



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
	public async static Task<CreateResponse> CreateOn(string key, int org, Conexión context)
	{

		// Ejecución
		try
		{

			// Query
			var app = await (from A in context.DataBase.Applications
							 where A.Key == key
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



}
