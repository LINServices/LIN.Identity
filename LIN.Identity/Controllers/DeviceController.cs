namespace LIN.Identity.Controllers;


[Route("devices")]
public class DeviceController : ControllerBase
{


    /// <summary>
    /// Obtiene la lista de dispositivos asociados a una cuenta en tiempo real
    /// </summary>
    /// <param name="token">Token de acceso</param>
    [HttpGet]
    public HttpReadAllResponse<DeviceModel> GetAll([FromHeader] string token)
    {
        try
        {

            var (isValid, _, userID, _) = Jwt.Validate(token);

            if (!isValid)
            {
                return new ReadAllResponse<DeviceModel>
                {
                    Message = "Token Invalido",
                    Response = Responses.Unauthorized
                };
            }


            var devices = (from A in AccountHub.Cuentas
                           where A.Key == userID
                           select A).FirstOrDefault().Value ?? new();


            var filter = (from D in devices
                          where D.Estado == DeviceState.Actived
                          select D).ToList();

            // Retorna
            return new(Responses.Success, filter ?? new());
        }
        catch
        {
            return new(Responses.Undefined);
        }
    }



}
