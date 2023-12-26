namespace LIN.Identity.Areas.Organizations;


[Route("organizations")]
public class OrganizationsController : ControllerBase
{


    /// <summary>
    /// Crea una nueva organización.
    /// </summary>
    /// <param name="modelo">Modelo de la organización y el usuario administrador</param>
    [HttpPost("create")]
    public async Task<HttpCreateResponse> Create([FromBody] OrganizationModel modelo)
    {

        // Validar el modelo.
        if (modelo == null || string.IsNullOrWhiteSpace(modelo.Name) || modelo.Directory == null || modelo.Directory.Identity == null || string.IsNullOrWhiteSpace(modelo.Directory.Identity.Unique))
            return new()
            {
                Response = Responses.InvalidParam,
                Message = "Parámetros inválidos."
            };


        // Ordenar el modelo.
        {
            modelo.ID = 0;
            modelo.Name = modelo.Name.Trim();
            modelo.Directory.Nombre = modelo.Directory.Nombre.Trim();
            modelo.Directory.Members = [];
            modelo.Directory.Policies = [];
            modelo.Directory.Creación = DateTime.Now;
            modelo.Directory.Identity.Type = IdentityTypes.Directory;
            modelo.Directory.Identity.DirectoryMembers = [];
            modelo.Directory.Identity.Unique = modelo.Directory.Identity.Unique.Trim();
        }


        // Creación de la organización.
        var response = await Data.Areas.Organizations.Organizations.Create(modelo);

        // Evaluación.
        if (response.Response != Responses.Success)
            return new(response.Response);

        // Retorna el resultado.
        return new CreateResponse()
        {
            LastID = response.Model.ID,
            Response = Responses.Success,
            Message = "Success"
        };

    }



    /// <summary>
    /// Obtiene una organización por medio del Id.
    /// </summary>
    /// <param name="id">ID de la organización</param>
    /// <param name="token">Token de acceso</param>
    [HttpGet("read/id")]
    public async Task<HttpReadOneResponse<OrganizationModel>> ReadOneByID([FromQuery] int id, [FromHeader] string token)
    {

        // Parámetros
        if (id <= 0)
            return new(Responses.InvalidParam);

        // Token.
        var tokenInfo = Jwt.Validate(token);

        // Si el token no es valido.
        if (!tokenInfo.IsAuthenticated)
            return new()
            {
                Response = Responses.Unauthorized,
                Message = "Token invalido."
            };


        // Obtiene la organización
        var response = await Data.Areas.Organizations.Organizations.Read(id);

        // Organización no encontrada.
        if (response.Response != Responses.Success)
            return new ReadOneResponse<OrganizationModel>()
            {
                Response = Responses.NotRows,
                Message = "No se encontró la organización."
            };

        // No es publica y no pertenece a ella
        if (!response.Model.IsPublic && orgID != response.Model.ID)
            return new ReadOneResponse<OrganizationModel>()
            {
                Response = Responses.Unauthorized,
                Message = "Esta organización es privada y tu usuario no esta vinculado a ella.",
                Model = new()
                {
                    ID = response.Model.ID,
                    IsPublic = false,
                    Name = "Organización privada"
                }
            };

        return new ReadOneResponse<OrganizationModel>()
        {
            Response = Responses.Success,
            Model = response.Model
        };


    }



}