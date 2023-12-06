namespace LIN.Identity.Areas.Applications;


[Route("applications")]
public class ApplicationController : ControllerBase
{


    /// <summary>
    /// Crear nueva aplicación.
    /// </summary>
    /// <param name="applicationModel">Modelo.</param>
    /// <param name="token">Token de acceso.</param>
    [HttpPost]
    public async Task<HttpCreateResponse> Create([FromBody] ApplicationModel applicationModel, [FromHeader] string token)
    {

        // Información del token.
        var (isValid, _, userID, _, _) = Jwt.Validate(token);

        // Si el token es invalido.
        if (!isValid)
            return new CreateResponse()
            {
                Response = Responses.Unauthorized,
                Message = "El token es invalido."
            };

        // Validaciones.
        if (applicationModel == null || applicationModel.ApplicationUid.Trim().Length < 4 || applicationModel.Name.Trim().Length < 4)
            return new CreateResponse()
            {
                Response = Responses.InvalidParam,
                Message = "Parámetros inválidos."
            };

        // Preparar el modelo
        applicationModel.ApplicationUid = applicationModel.ApplicationUid.Trim().ToLower();
        applicationModel.Name = applicationModel.Name.Trim().ToLower();
        applicationModel.AccountID = userID;

        // Crear la aplicación.
        return await Data.Applications.Create(applicationModel);

    }



    /// <summary>
    /// Obtener las aplicaciones asociadas
    /// </summary>
    /// <param name="token">Token de acceso</param>
    [HttpGet]
    public async Task<HttpReadAllResponse<ApplicationModel>> GetAll([FromHeader] string token)
    {

        // Información del token.
        var (isValid, _, userID, _, _) = Jwt.Validate(token);

        // Si el token es invalido.
        if (!isValid)
            return new ReadAllResponse<ApplicationModel>()
            {
                Response = Responses.Unauthorized,
                Message = "El token es invalido."
            };

        // Obtiene la data.
        var data = await Data.Applications.ReadAll(userID);

        return data;

    }




}