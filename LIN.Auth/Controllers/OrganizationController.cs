namespace LIN.Auth.Controllers;


[Route("orgs")]
public class OrganizationsController : ControllerBase
{



    /// <summary>
    /// Crea una organización
    /// </summary>
    /// <param name="modelo">Modelo de la organización</param>
    [HttpPost("create")]
    public async Task<HttpCreateResponse> Create([FromBody] OrganizationModel modelo, [FromHeader] string token)
    {

        // Comprobaciones
        if (modelo == null || modelo.Domain.Length <= 0 || modelo.Name.Length <= 0)
            return new(Responses.InvalidParam);

        // Token
        var (isValid, _, userID) = Jwt.Validate(token);

        // Validación del token
        if (!isValid)
            return new CreateResponse()
            {
                Response = Responses.Unauthorized,
                Message = "Token invalido"
            };

        // Obtiene la cuenta
        var account = await Data.Accounts.Read(userID, true, true, true);

        // Validación de la cuenta
        if (account.Response != Responses.Success)
        {
            return new CreateResponse()
            {
                Response = Responses.Unauthorized,
                Message = "No se encontró el usuario"
            };
        }
        

        // Si ya el usuario tiene organización
        if (account.Model.Organization != null)
        {
            return new CreateResponse()
            {
                Response = Responses.UnauthorizedByOrg,
                Message = "Ya perteneces a una organización."
            };
        }


        // Organización del modelo
        modelo.ID = 0;
        modelo.AppList = Array.Empty<AppOrganizationModel>();
        modelo.Members = Array.Empty<AccountModel>();


        // Conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();


        modelo.Members.Add(account.Model);

        // Creación de la organización
        var response = await Data.Organizations.Create(modelo, context);

        // Evaluación
        if (response.Response != Responses.Success)
            return new(response.Response);


        context.CloseActions(connectionKey);

        // Retorna el resultado
        return new CreateResponse()
        {
            LastID = response.LastID,
            Response = Responses.Success,
            Message = "Success"
        };

    }



    /// <summary>
    /// Obtiene una organización por medio del ID
    /// </summary>
    /// <param name="id">ID de la organización</param>
    [HttpGet("read/id")]
    public async Task<HttpReadOneResponse<OrganizationModel>> ReadOneByID([FromQuery] int id)
    {

        if (id <= 0)
            return new(Responses.InvalidParam);

        // Obtiene el usuario
        var response = await Data.Organizations.Read(id);

        // Si es erróneo
        if (response.Response != Responses.Success)
            return new ReadOneResponse<OrganizationModel>()
            {
                Response = response.Response,
                Model = new()
            };

        // Retorna el resultado
        return response;

    }



}