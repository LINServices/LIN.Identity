namespace LIN.Auth.Controllers;


[Route("/")]
public class APIVersion : Controller
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





    [HttpGet]
    public dynamic Index()
    {
        return Ok("Abierto");
    }


}