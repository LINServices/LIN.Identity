namespace LIN.Identity.Controllers;


[Route("/")]
public class ApiVersion : Controller
{


    /// <summary>
    /// Obtiene el estado del servidor
    /// </summary>
    [HttpGet("status")]
    public dynamic Status()
    {
        return StatusCode(200, new
        {
            Status = "Open"
        });
    }


}