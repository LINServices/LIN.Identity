namespace LIN.Cloud.Identity.Areas.Organizations;

[Route("[controller]")]
public class OrganizationsController(Data.Organizations organizationsData, Data.DirectoryMembers directoryMembersData) : AuthenticationBaseController
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
            modelo.Id = 0;
            modelo.Name = modelo.Name.Trim();
            modelo.Creation = DateTime.Now;
            modelo.Directory.Members = [];
            modelo.Directory.Name = modelo.Directory.Name.Trim();
            modelo.Directory.Identity.EffectiveTime = DateTime.Now;
            modelo.Directory.Identity.CreationTime = DateTime.Now;
            modelo.Directory.Identity.EffectiveTime = DateTime.Now.AddYears(10);
            modelo.Directory.Identity.Status = IdentityStatus.Enable;
        }

        // Creación de la organización.
        var response = await organizationsData.Create(modelo);

        // Evaluación.
        if (response.Response != Responses.Success)
            return new(response.Response);

        // Retorna el resultado.
        return new CreateResponse()
        {
            LastID = response.LastID,
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
    [IdentityToken]
    public async Task<HttpReadOneResponse<OrganizationModel>> ReadOneByID([FromQuery] int id)
    {

        // Parámetros
        if (id <= 0)
            return new(Responses.InvalidParam);

        // Obtiene la organización
        var response = await organizationsData.Read(id);

        // Organización no encontrada.
        if (response.Response != Responses.Success)
            return new ReadOneResponse<OrganizationModel>()
            {
                Response = Responses.NotRows,
                Message = "No se encontró la organización."
            };

        // No es publica y no pertenece a ella
        if (!response.Model.IsPublic)
        {

            var iamIn = await directoryMembersData.IamIn(AuthenticationInformation.IdentityId, response.Model.Id);

            if (iamIn.Response != Responses.Success)
                return new ReadOneResponse<OrganizationModel>()
                {
                    Response = Responses.Unauthorized,
                    Message = "Esta organización es privada y tu usuario no esta vinculado a ella.",
                    Model = new()
                    {
                        Id = response.Model.Id,
                        IsPublic = false,
                        Name = "Organización privada"
                    }
                };
        }

        return new ReadOneResponse<OrganizationModel>()
        {
            Response = Responses.Success,
            Model = response.Model
        };

    }


    /// <summary>
    /// Obtiene las organizaciones donde un usuario es miembro.
    /// </summary>
    /// <param name="token">Token de acceso</param>
    [HttpGet("read/all")]
    [IdentityToken]
    public async Task<HttpReadAllResponse<OrganizationModel>> ReadAll()
    {

        // Obtiene la organización
        var response = await organizationsData.ReadAll(AuthenticationInformation.IdentityId);

        return response;

    }

}