using LIN.Identity.Data.Areas.Directories;

namespace LIN.Identity.Areas.Directories;


[Route("directory/members")]
public class DirectoryMembersController : ControllerBase
{


    [HttpPost]
    [TokenAuth]
    public async Task<HttpCreateResponse> Create([FromHeader] string token, [FromBody] DirectoryMember model)
    {

        // Token.
        JwtModel tokenInfo = HttpContext.Items["token"] as JwtModel ?? new();

        // Acceso IAM.
        var (_, _, roles) = await Data.Queries.Directories.Get(tokenInfo.IdentityId);

        // Si no hay acceso.
        if (Roles.AlterMembers(roles))
            return new()
            {
                Response = Responses.Unauthorized,
                Message = "No tienes permisos para administrar los integrantes de este recurso."
            };



        var guestIdentity = await Data.Identities.Read(model.Identity.Id);

        if (guestIdentity.Response != Responses.Success)
            return new()
            {
                Message = "Error",
                Response = Responses.NotRows
            };


        // Obtiene el usuario.
        var response = await DirectoryMembers.Create(model);


        // Retorna el resultado
        return response;

    }



}