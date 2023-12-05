namespace LIN.Identity.Areas.V1.Accounts;


[Route("v1/devices")]
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

            var (isValid, _, userId, _, _) = Jwt.Validate(token);

            if (!isValid)
            {
                return new ReadAllResponse<DeviceModel>
                {
                    Message = "Token Invalido",
                    Response = Responses.Unauthorized
                };
            }


            var devices = (from account in AccountHub.Cuentas
                           where account.Key == userId
                           select account).FirstOrDefault().Value ?? new();


            var filter = (from device in devices
                          where device.Estado == DeviceState.Actived
                          select device).ToList();

            // Retorna
            return new(Responses.Success, filter ?? new());
        }
        catch
        {
            return new(Responses.Undefined)
            {
                Message = "Hubo un error al obtener los dispositivos asociados."
            };
        }
    }



}