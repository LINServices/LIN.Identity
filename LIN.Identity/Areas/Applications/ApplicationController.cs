namespace LIN.Identity.Areas.Applications;


[Route("applications")]
public class ApplicationController : ControllerBase
{


    /// <summary>
    /// Crear nueva aplicaci�n.
    /// </summary>
    /// <param name="applicationModel">Modelo.</param>
    /// <param name="token">Token de acceso.</param>
    [HttpPost]
    public async Task<HttpCreateResponse> Create([FromBody] ApplicationModel applicationModel, [FromHeader] string token)
    {

        // Informaci�n del token.
        var (isValid, _, userID, _, _, _) = Jwt.Validate(token);;

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
                Message = "Par�metros inv�lidos."
            };

        // Preparar el modelo
        applicationModel.ApplicationUid = applicationModel.ApplicationUid.Trim().ToLower();
        applicationModel.Name = applicationModel.Name.Trim().ToLower();
        applicationModel.AccountID = userID;

        // Crear la aplicaci�n.
        return await Data.Applications.Create(applicationModel);

    }



    /// <summary>
    /// Obtener las aplicaciones asociadas
    /// </summary>
    /// <param name="token">Token de acceso</param>
    [HttpGet]
    public async Task<HttpReadAllResponse<ApplicationModel>> GetAll([FromHeader] string token)
    {

        // Informaci�n del token.
        var (isValid, _, userID, _, _, _) = Jwt.Validate(token);;

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



    /// <summary>
    /// Crear acceso permitido a una app.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    /// <param name="appId">ID de la aplicaci�n.</param>
    /// <param name="accountId">ID del integrante.</param>
    [HttpPut]
    public async Task<HttpReadOneResponse<bool>> InsertAllow([FromHeader] string token, [FromHeader] int appId, [FromHeader] int accountId)
    {

        // Informaci�n del token.
        var (isValid, _, userId, _, _, _) = Jwt.Validate(token);;

        // Si el token es invalido.
        if (!isValid)
            return new ReadOneResponse<bool>()
            {
                Response = Responses.Unauthorized,
                Message = "El token es invalido."
            };

        // Respuesta de Iam.
        var iam = await Services.Iam.Applications.ValidateAccess(userId, appId);

        // Validaci�n de Iam
        if (iam.Model != IamLevels.Privileged)
            return new ReadOneResponse<bool>()
            {
                Response = Responses.Unauthorized,
                Message = "No tienes autorizaci�n para modificar este recurso."
            };


        // Enviar la actualizaci�n
        var data = await Data.Applications.AllowTo(appId, accountId);

        return data;

    }



}